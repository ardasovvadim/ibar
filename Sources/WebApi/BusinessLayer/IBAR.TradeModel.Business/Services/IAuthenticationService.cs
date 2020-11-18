using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.Providers;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface IAuthenticationService
    {
        UserModel LoginAs(LoginModel modelParam, string roleName);
        UserModel ConfirmLogin(ConfirmModel modelParam, string roleName);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogInfoService _logInfoService;
        private readonly IMessengerService _messengerService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IMapper _mapper;
        private readonly IActionContextAccessor _actionContext;
        private IEnumerable<string> Domains { get; set; }

        public AuthenticationService(IUserRepository userRepository, ILogInfoService logInfoService,
            IMessengerService messengerService, IJwtTokenProvider jwtTokenProvider, IMapper mapper, IActionContextAccessor actionContext)
        {
            _userRepository = userRepository;
            _logInfoService = logInfoService;
            _messengerService = messengerService;
            _jwtTokenProvider = jwtTokenProvider;
            _mapper = mapper;
            _actionContext = actionContext;
        }

        public UserModel LoginAs(LoginModel modelParam, string roleName)
        {
            // TODO: ardasovvadim: encrypt and decrypt password
            
            ValidateDomain(modelParam.Email);
            _actionContext.ThrowIfModelInvalid();
            
            var repositoryUser = _userRepository.GetByEmail(modelParam.Email);

            CheckCredentials(modelParam, repositoryUser, roleName);
            _actionContext.ThrowIfModelInvalid();

            var verificationCode = SecurityExtensions.GetVerificationCode();

            _logInfoService.AddVerificationLog(repositoryUser, verificationCode);

            _messengerService.SendSms(repositoryUser.Phone, $"Your Code: {verificationCode}");

            return _mapper.Map<UserModel>(repositoryUser);
        }

        public UserModel ConfirmLogin(ConfirmModel modelParam, string roleName)
        {
            ValidateDomain(modelParam.Email);
            _actionContext.ThrowIfModelInvalid();
            
            var repositoryUser = _userRepository.GetByEmail(modelParam.Email);

            CheckCredentials(modelParam, repositoryUser, roleName);
            _actionContext.ThrowIfModelInvalid();

            _logInfoService.CheckAndLogin(repositoryUser, modelParam.VerificationCode);

            repositoryUser.AccessToken = _jwtTokenProvider.GenerateAccessToken(repositoryUser);
            _userRepository.Save(repositoryUser);

            return _mapper.Map<UserModel>(repositoryUser);
        }

        private void CheckCredentials(LoginModel modelParam, User repositoryUser, string roleName)
        {
            if (repositoryUser == null ||
                modelParam.Password != repositoryUser.Password ||
                repositoryUser.Roles.All(role => role.Name != roleName))
            {
                _actionContext.SetModelError("invalidLoginData", "Invalid login Data");
            }
         
            if (repositoryUser != null && repositoryUser.IsWaitingConfirmation)
            {
                _actionContext.SetModelError("invalidLoginData", "Account is waiting confirmation");
            }
        }
        
        #region Private

        private void ValidateDomain(string email)
        {
            if (Domains == null)
                Domains = ConfigurationManager.AppSettings["domains"].Split(';');

            var emailDomain = email.Split('@')[1];
            if (!Domains.Any(domain => string.Equals(domain, emailDomain, StringComparison.OrdinalIgnoreCase)))
                _actionContext.SetModelError("email", "Invalid email domain");
        }

        #endregion
    }
}