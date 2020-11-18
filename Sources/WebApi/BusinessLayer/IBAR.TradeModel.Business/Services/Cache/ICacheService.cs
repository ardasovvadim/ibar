using System;
using System.Collections.Generic;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services.Cache
{
    public interface ICacheService
    {
        IEnumerable<(string TradeAccountName, long Id)> TradeAccounts();
    }
    
    internal class CacheService: ICacheService
    {
        private readonly IImportJobRepository _importJobRepository;

        public CacheService(IImportJobRepository importJobRepository)
        {
            _importJobRepository = importJobRepository;
        }

        IEnumerable<(string TradeAccountName, long Id)> ICacheService.TradeAccounts()
        {
            // var result = _tradeRepository.Current<MasterAccount>().Get().Select(a => (a.AccountName, a.Id)).ToArray();
            throw new NotImplementedException("TradeAccounts in ICache service");
        }
    }

}
