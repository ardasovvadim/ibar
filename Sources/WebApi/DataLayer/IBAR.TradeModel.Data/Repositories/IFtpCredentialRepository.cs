using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IFtpCredentialRepository
    {
        bool IsExists(FtpCredential dto);
        FtpCredential Add(FtpCredential dto);
        FtpCredential GetById(long id);
        FtpCredential Update(FtpCredential cred);
        IEnumerable<FtpCredential> GetAll();
        IQueryable<FtpCredential> Query();
    }

    public class FtpCredentialRepository : IFtpCredentialRepository
    {
        private readonly TradeModelContext _dbContext;

        public FtpCredentialRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsExists(FtpCredential dto)
        {
            return _dbContext
                    .FtpCredentials
                    .Any(cred => !cred.Deleted && (cred.Id == dto.Id ||cred.FtpName == dto.FtpName));
        }

        public FtpCredential Add(FtpCredential dto)
        {
            dto = _dbContext.FtpCredentials.Add(dto);
            _dbContext.SaveChanges();
            return dto;
        }

        public FtpCredential GetById(long id)
        {
            return _dbContext
                    .FtpCredentials
                    .Include(x => x.MasterAccounts)
                    .FirstOrDefault(cred => cred.Id == id);
        }

        public FtpCredential Update(FtpCredential cred)
        {
            _dbContext.Entry(cred).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return cred;
        }

        public IEnumerable<FtpCredential> GetAll()
        {
            return _dbContext
                    .FtpCredentials
                    .Include(x => x.MasterAccounts)
                    .Where(cred => !cred.Deleted);
        }

        public IQueryable<FtpCredential> Query()
        {
            return _dbContext.FtpCredentials.Where(cred => !cred.Deleted);
        }
    }
}