using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using IBAR.Api.Models;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface IUserService
    {
        IEnumerable<UserGridViewModel> GetAllUsers();
        UserModel GetById(long id);
        long ChangeRole(UserRoleModel modelParam);
        long AddNewUser(UserCreateEditModel modelParam);
        long DeleteUser(long id);
        long UpdateUser(UserCreateEditModel modelParam);
        void ResendInvite(long id);
        void ChangePassword(ChangePasswordEditModel model);
    }

    // TODO: after refactoring db. need get rid of the UserModel.cs 
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IInviteService _inviteService;
        private readonly IMapper _mapper;
        private readonly IActionContextAccessor _actionContext;

        private IEnumerable<string> Domains { get; set; }

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IInviteService inviteService,
            IMapper mapper, IActionContextAccessor actionContext)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _inviteService = inviteService;
            _mapper = mapper;
            _actionContext = actionContext;
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

        public UserModel GetById(long id)
        {
            return _mapper.Map<UserModel>(_userRepository.GetById(id));
        }

        public long ChangeRole(UserRoleModel modelParam)
        {
            // TODO: need add constraint in db and throw exception
            var dbUser = _userRepository.Query()
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Id == modelParam.Id);

            if (dbUser == null)
            {
                _actionContext.SetModelError("idUser", "User not found by id");
                _actionContext.ThrowIfModelInvalid();
            }

            var userRoles = dbUser.Roles.Select(r => r.Name).ToList();
            var allRoles = _roleRepository.GetAll();
            
            // Check is valid client role list
            if (userRoles.Any(role => !allRoles.Select(r => r.Name).Contains(role)))
            {
                _actionContext.SetModelError("roles", "Invalid role list");
                _actionContext.ThrowIfModelInvalid();
            }

            // Add roles
            var newRoles = modelParam.Roles.Except(userRoles).ToList();
            if (newRoles.Any())
            {
                var additional = allRoles.Where(r => newRoles.Contains(r.Name)).ToList();
                additional.ForEach(add => dbUser.Roles.Add(add));
            }

            // Minus roles
            var removeRoles = userRoles.Except(modelParam.Roles).ToList();
            if (removeRoles.Any())
            {
                var forDeleting = allRoles.Where(r => removeRoles.Contains(r.Name)).ToList();
                dbUser.Roles = dbUser.Roles.Except(forDeleting).ToList();
            }

            _userRepository.Save(dbUser);

            return modelParam.Id;
        }

        public long AddNewUser(UserCreateEditModel modelParam)
        {
            // TODO: need refactor after changes related to Code First db
            ValidateDomain(modelParam.Email);

            var user = _mapper.Map<User>(modelParam);

            var entry = _userRepository.GetEntry(user);
            if (entry != null)
            {
                if (entry.Email == user.Email)
                    _actionContext.SetModelError("email", "Email is busy");
                if (entry.Phone == user.Phone)
                    _actionContext.SetModelError("phone", "Phone is busy");
            }

            _actionContext.ThrowIfModelInvalid();

            user.Roles.Add(_roleRepository.GetDefault());
            user.IsWaitingConfirmation = true;
            user = _userRepository.Add(user);

            _inviteService.InviteUser(user);

            return user.Id;
        }

        public long DeleteUser(long id)
        {
            if (!_userRepository.IsExists(new User {Id = id}))
                throw new UserNotFoundException();

            _userRepository.Delete(id);

            return id;
        }

        public long UpdateUser(UserCreateEditModel modelParam)
        {
            // TODO: need refactor after changes related to Code First db
            ValidateDomain(modelParam.Email);

            var user = _mapper.Map<User>(modelParam);

            if (!_userRepository.IsExists(new User{Id = user.Id})) 
                throw new UserNotFoundException();

            var entry = _userRepository.GetEntry(new User {Email = user.Email});
            if (entry != null && entry.Id != user.Id)
                _actionContext.SetModelError("email", "Email is busy");

            entry = _userRepository.GetEntry(new User {Phone = user.Phone});
            if (entry != null && entry.Id != user.Id)
                _actionContext.SetModelError("phone", "Phone is busy");
            
            _actionContext.ThrowIfModelInvalid();
            
            user = _userRepository.Update(user);
            
            return user.Id;
        }

        public void ResendInvite(long id)
        {
            var user = _userRepository.Query()
                .FirstOrDefault(u => !u.Deleted && u.IsWaitingConfirmation && u.Id == id);

            if (user == null) throw new EntityNotFoundException("User not found by id");

            _inviteService.ExpireInvite(id);
            _inviteService.InviteUser(user);
        }

        public void ChangePassword(ChangePasswordEditModel model)
        {
            var user = _userRepository.GetById(model.Id);

            if (user == null || user.Password != model.OldPassword)
                _actionContext.SetModelErrorAndThrow("oldPassword", "Invalid password");
            
            user.Password = model.NewPassword;
            _userRepository.Save(user);
        }

        public IEnumerable<UserGridViewModel> GetAllUsers()
        {
            var users = _userRepository.Query().Include(u => u.Roles).ToList();
            return _mapper.Map<IEnumerable<UserGridViewModel>>(users);
        }
    }
}