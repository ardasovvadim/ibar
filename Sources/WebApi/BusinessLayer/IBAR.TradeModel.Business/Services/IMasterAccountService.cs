using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using IBAR.ServiceLayer.ViewModels;
using IBAR.TradeModel.Business.Common;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.Utils;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Business.ViewModels.Response.Admin;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface IMasterAccountService
    {
        IEnumerable<MasterAccountGridVm> GetAllMasterAccountGridVm();
        long AddMasterAccount(MasterAccountCreateEditModel modelParam);
        long DeleteMasterAccount(long id);
        long UpdateMasterAccount(MasterAccountCreateEditModel modelParam);
        MasterAccountVm GetById(long id);
        IEnumerable<MasterAccountVm> GetAll();
        IEnumerable<IdNameModel> GetIdNames();
    }

    public class MasterAccountService : IMasterAccountService
    {
        private readonly IMasterAccountRepository _masterAccountRepository;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ITradeAccountRepository _tradeAccountRepository;
        private readonly IActionContextAccessor _actionContext;

        public MasterAccountService(IMasterAccountRepository masterAccountRepository,
            IIdentityService identityService, IMapper mapper, ITradeAccountRepository tradeAccountRepository,
            IActionContextAccessor actionContext)
        {
            _masterAccountRepository = masterAccountRepository;
            _identityService = identityService;
            _mapper = mapper;
            _tradeAccountRepository = tradeAccountRepository;
            _actionContext = actionContext;
        }

        public IEnumerable<MasterAccountGridVm> GetAllMasterAccountGridVm()
        {
            var accounts = _mapper.Map<List<MasterAccountGridVm>>(_masterAccountRepository.Query()
                .Where(acc => !acc.Deleted)
                .Include(acc => acc.UpdatedBy)
                .Include(acc => acc.CreatedBy).ToList());

            accounts.ForEach(acc => acc.AmountTradeAccounts = _tradeAccountRepository.Query()
                .Include(a => a.MasterAccountId)
                .Count(a => a.MasterAccountId == acc.Id));

            return accounts;
        }

        public long AddMasterAccount(MasterAccountCreateEditModel modelParam)
        {
            var dto = _mapper.Map<MasterAccount>(modelParam);

            var entry = _masterAccountRepository.Query().FirstOrDefault(acc =>
                acc.Deleted && acc.AccountName == dto.AccountName);
            // restore existing account
            if (entry != null)
            {
                var existingAccount = entry;

                entry = _masterAccountRepository.GetEntry(new MasterAccount {AccountAlias = dto.AccountAlias});
                if (entry != null) _actionContext.SetModelError("accountAlias", "Account alias is busy");
                _actionContext.ThrowIfModelInvalid();

                existingAccount.Deleted = false;
                existingAccount.AccountAlias = dto.AccountAlias;
                existingAccount.UpdatedById = _identityService.GetIdentityId();
                return _masterAccountRepository.Update(existingAccount).Id;
            }

            entry = _masterAccountRepository.GetEntry(new MasterAccount {AccountAlias = dto.AccountAlias});
            if (entry != null) _actionContext.SetModelError("accountAlias", "Account alias is busy");

            entry = _masterAccountRepository.GetEntry(new MasterAccount {AccountName = dto.AccountName});
            if (entry != null) _actionContext.SetModelError("accountName", "Account name is busy");

            _actionContext.ThrowIfModelInvalid();

            dto.CreatedById = _identityService.GetIdentityId();

            dto = _masterAccountRepository.Add(dto);

            return dto.Id;
        }

        public long DeleteMasterAccount(long id)
        {
            if (!_masterAccountRepository.IsExists(new MasterAccount {Id = id}))
                throw new MasterAccountNotFoundException();

            var account = _masterAccountRepository.GetById(id);

            account.Deleted = true;
            account.UpdatedById = _identityService.GetIdentityId();

            _masterAccountRepository.Update(account);

            return id;
        }

        public long UpdateMasterAccount(MasterAccountCreateEditModel modelParam)
        {
            var dto = _mapper.Map<MasterAccount>(modelParam);

            if (!_masterAccountRepository.IsExists(new MasterAccount {Id = dto.Id}))
                throw new MasterAccountNotFoundException();

            var entry = _masterAccountRepository.GetEntry(new MasterAccount {AccountAlias = dto.AccountAlias});
            if (entry != null && entry.Id != dto.Id)
                _actionContext.SetModelError("accountAlias", "Account alias is busy");

            entry = _masterAccountRepository.GetEntry(new MasterAccount {AccountName = dto.AccountName});
            if (entry != null && entry.Id != dto.Id)
                _actionContext.SetModelError("accountName", "Account name is busy");

            _actionContext.ThrowIfModelInvalid();

            var existingAccount = _masterAccountRepository.GetById(dto.Id);
            existingAccount.UpdatedById = _identityService.GetIdentityId();
            existingAccount.AccountName = modelParam.AccountName;
            existingAccount.AccountAlias = modelParam.AccountAlias;

            existingAccount = _masterAccountRepository.Update(existingAccount);

            return existingAccount.Id;
        }

        public MasterAccountVm GetById(long id)
        {
            return _mapper.Map<MasterAccountVm>(_masterAccountRepository.GetById(id));
        }

        public IEnumerable<MasterAccountVm> GetAll()
        {
            return _mapper.Map<IEnumerable<MasterAccountVm>>(_masterAccountRepository.GetAll());
        }

        public IEnumerable<IdNameModel> GetIdNames()
        {
            return _masterAccountRepository.GetAll().Select(acc => new IdNameModel
            {
                Id = acc.Id,
                Name = TradeUtils.ResolveMasterAccountName(acc)
            });
        }
    }
}