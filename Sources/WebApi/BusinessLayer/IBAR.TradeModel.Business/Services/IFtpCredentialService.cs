using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IBAR.TradeModel.Business.Exceptions;
using IBAR.TradeModel.Business.ViewModels.Request.Admin;
using IBAR.TradeModel.Business.ViewModels.Response;
using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface IFtpCredentialService
    {
        IEnumerable<FtpCredentialGridVm> GetAll();
        long AddNewFtpCredential(FtpCredentialCreateEditModel gridVm);
        long DeleteFtpCredential(long id);
        long UpdateFtpCredential(FtpCredentialCreateEditModel model);
        IEnumerable<IdNameModel> GetIdNames();
    }

    public class FtpCredentialService : IFtpCredentialService
    {
        private readonly IFtpCredentialRepository _ftpCredentialRepository;
        private readonly IMasterAccountRepository _masterAccountRepository;
        private readonly IMapper _mapper;

        public FtpCredentialService(IFtpCredentialRepository ftpCredentialRepository
            , IMasterAccountRepository masterAccountRepository, IMapper mapper)
        {
            _ftpCredentialRepository = ftpCredentialRepository;
            _masterAccountRepository = masterAccountRepository;
            _mapper = mapper;
        }

        public IEnumerable<FtpCredentialGridVm> GetAll()
        {
            return _mapper.Map<IEnumerable<FtpCredentialGridVm>>(_ftpCredentialRepository.GetAll());
        }

        public long AddNewFtpCredential(FtpCredentialCreateEditModel model)
        {
            var dto = _mapper.Map<FtpCredential>(model);
            dto.MasterAccounts.Clear();

            if (_ftpCredentialRepository.IsExists(dto)) 
                throw new FtpCredentialAlreadyExistsException();
            
            model.MasterAccounts.ToList().ForEach(acc =>
            {
                var masterAcc = _masterAccountRepository.GetById(acc.Id);
                if (masterAcc == null) throw new MasterAccountNotFoundException();
                dto.MasterAccounts.Add(masterAcc);
            });
            
            dto = _ftpCredentialRepository.Add(dto);
            
            return dto.Id;
        }

        public long DeleteFtpCredential(long id)
        {
            var cred = _ftpCredentialRepository.GetById(id);

            if (cred == null) 
                throw new FtpCredentialNotFoundException();

            cred.Deleted = true;

            _ftpCredentialRepository.Update(cred);

            return id;
        }

        public long UpdateFtpCredential(FtpCredentialCreateEditModel model)
        {
            var dto = _ftpCredentialRepository.GetById(model.Id);

            if (dto == null)
                throw new FtpCredentialNotFoundException();

            dto.FtpName = model.FtpName;
            dto.UserName = model.UserName;
            dto.UserPassword = model.UserPassword;
            dto.Url = model.Url;

            var exists = dto.MasterAccounts.Select(acc => acc.Id).ToList();
            var newMasterAccs = model.MasterAccounts.Select(acc => acc.Id).ToList();
            var forDeleting = exists.Except(newMasterAccs).ToList();
            var forAdding = newMasterAccs.Except(exists).ToList();

            forDeleting.ForEach(id =>
            {
                var account = dto.MasterAccounts.FirstOrDefault(acc => acc.Id == id);
                if (account == null) throw new MasterAccountNotFoundException();

                dto.MasterAccounts.Remove(account);
            });

            forAdding.ForEach(id =>
            {
                var account = _masterAccountRepository.GetById(id);
                if (account == null) throw new MasterAccountNotFoundException();

                dto.MasterAccounts.Add(account);
            });

            dto = _ftpCredentialRepository.Update(dto);

            return dto.Id;
        }

        public IEnumerable<IdNameModel> GetIdNames()
        {
            return _ftpCredentialRepository.Query().Select(cred => new IdNameModel {Id = cred.Id, Name = cred.FtpName});
        }
    }
}