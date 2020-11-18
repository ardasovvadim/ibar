using AutoMapper;
using IBAR.ServiceLayer.Common;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace IBAR.TradeModel.Business.Services
{
    public interface IInviteService
    {
        string BaseUrl { get; }
        Invite InviteUser(User user);
        void SendRegistrationCode(string linkKey);
        UserModel ConfirmRegistration(string linkKey, int phoneCode);
        void FinishRegistration(string linkKey, LoginModel modelParam);
        bool IsWaitingConfirmation(string linkKey);
        void ExpireInvite(long id);
    }

    public class InviteService : IInviteService
    {
        private readonly IInviteRepository _inviteRepository;
        private readonly IMessengerService _messengerService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private IActionContextAccessor ContextAccessor { get; }

        public string BaseUrl { get; }

        public InviteService(IInviteRepository inviteRepository, IMessengerService messengerService,
            IUserRepository userRepository, IMapper mapper, IActionContextAccessor contextAccessor)
        {
            _inviteRepository = inviteRepository;
            _messengerService = messengerService;
            _userRepository = userRepository;
            _mapper = mapper;
            ContextAccessor = contextAccessor;

            BaseUrl = ConfigurationManager.AppSettings["baseUrlWebApp"];
            if (string.IsNullOrEmpty(BaseUrl))
                throw new ConfigurationErrorsException("Please add 'baseUrlWebApp' settigns to .config file.");
        }

        public Invite InviteUser(User user)
        {
            var invite = new Invite
            {
                IdUser = user.Id,
                LinkKey = Guid.NewGuid().ToString().Replace("-", ""),
                IsUsed = false,
                ExpiryDate = DateTime.UtcNow.AddHours(1)
            };
            _inviteRepository.Add(invite);

            var baseUrl = BaseUrl;
            var message =
                $"You are invited to Mexem reporting system. Please follow a link to get registered: <a href=\"{baseUrl}/registration/confirm/{invite.LinkKey}\">link</a>";
            var title = "IBAR App Confirm registration";
            _messengerService.SendEmail(user.Email, message, title, true);

            return invite;
        }

        public void SendRegistrationCode(string linkKey)
        {
            var invite = _inviteRepository.GetByLinkKey(linkKey);
            CheckInvite(invite);
            if (invite.PhoneCode != 0)
            {
                ContextAccessor.SetModelError("phoneCode", "Code has been sent yet");
                ContextAccessor.ThrowIfModelInvalid();
            }

            var verificationCode = SecurityExtensions.GetVerificationCode();
            var user = _userRepository.GetById(invite.IdUser);
            _messengerService.SendSms(user.Phone, $"Your code: {verificationCode}");
            invite.PhoneCode = verificationCode;
            _inviteRepository.Save(invite);
        }

        private void CheckInvite(Invite invite)
        {
            if (invite == null || invite.ExpiryDate < DateTime.UtcNow || invite.IsUsed)
            {
                ContextAccessor.SetModelError("linkkey", "Invalid or expired link");
                ContextAccessor.ThrowIfModelInvalid();
            }
        }

        public UserModel ConfirmRegistration(string linkKey, int phoneCode)
        {
            var invite = _inviteRepository.GetByLinkKey(linkKey);
            CheckInvite(invite);
            if (invite.PhoneCode != phoneCode)
            {
                ContextAccessor.SetModelError("phoneCode", "Invalid phone code");
                ContextAccessor.ThrowIfModelInvalid();
            }

            invite.IsConfirmPhoneCode = true;
            _inviteRepository.Save(invite);
            return _mapper.Map<UserModel>(_userRepository.GetById(invite.IdUser));
        }

        public void FinishRegistration(string linkKey, LoginModel modelParam)
        {
            var invite = _inviteRepository.GetByLinkKey(linkKey);

            CheckInvite(invite);

            if (!invite.IsConfirmPhoneCode)
            {
                ContextAccessor.SetModelErrorAndThrow("codePhone", "Code phone isn\'t confirm");
            }

            var dto = _userRepository.GetByEmail(modelParam.Email);

            if (dto == null)
                throw new UserNotFoundException();
            if (!Regex.IsMatch(modelParam.Password, Patterns.PasswordPattern))
            {
                ContextAccessor.SetModelErrorAndThrow("password", "Invalid password format");
            }

            dto.Password = modelParam.Password;
            dto.IsWaitingConfirmation = false;
            _userRepository.Save(dto);

            invite.IsUsed = true;
            _inviteRepository.Save(invite);
        }

        public bool IsWaitingConfirmation(string linkKey)
        {
            var invite = _inviteRepository.GetByLinkKey(linkKey);
            try
            {
                CheckInvite(invite);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ExpireInvite(long id)
        {
            var invite = _inviteRepository.Query()
                .Where(inv => !inv.Deleted && !inv.IsUsed && inv.IdUser == id)
                .OrderByDescending(inv => inv.ExpiryDate)
                .FirstOrDefault();

            if (invite == null) throw new EndpointNotFoundException("Can not get invite by user id");

            invite.IsUsed = true;
            _inviteRepository.Save(invite);
        }
    }
}