using System;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface ILogInfoService
    {
        LogInfo AddVerificationLog(User user, int verificationCode);
        void CheckAndLogin(User user, int verificationCode);
    }

    public class LogInfoService : ILogInfoService
    {
        private readonly ILogInfoRepository _logInfoRepository;
        private readonly IActionContextAccessor _actionContext;

        public LogInfoService(ILogInfoRepository logInfoRepository, IActionContextAccessor actionContext)
        {
            _logInfoRepository = logInfoRepository;
            _actionContext = actionContext;
        }

        public LogInfo AddVerificationLog(User user, int verificationCode)
        {
            var logInfo = new LogInfo();
            logInfo.User = user;
            logInfo.VerificationCode = verificationCode;
            logInfo.ExpiryDate = DateTime.UtcNow.AddMinutes(5);
            return _logInfoRepository.Add(logInfo);
        }

        public void CheckAndLogin(User user, int verificationCode)
        {
            var loginInfo = _logInfoRepository.GetLastByUserId(user.Id);
            if (loginInfo == null || loginInfo.IsUsed || loginInfo.ExpiryDate < DateTime.UtcNow)
                _actionContext.SetModelError("phoneCode", "Verification code was used or expired");
            if (loginInfo.VerificationCode != verificationCode)
                _actionContext.SetModelError("phoneCode", "Verification code invalid");

            _actionContext.ThrowIfModelInvalid();

            loginInfo.IsUsed = true;
            loginInfo.LoginTime = DateTime.UtcNow;
            _logInfoRepository.Save(loginInfo);
        }
    }

}