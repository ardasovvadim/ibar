using IBAR.TradeModel.Data.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IFtpJobRepository : IBaseJobRepository
    {
        IEnumerable<FtpCredential> GetFtpCredentialList();
        ICollection<ImportedFile> ImportedFileList();
        IEnumerable<TransitFiles> GetTransitFileList();
        ImportedFile AddImportedFile(ImportedFile importedFile);
    }

    public class FtpJobRepository : BaseRepository, IFtpJobRepository
    {
        public IEnumerable<FtpCredential> GetFtpCredentialList()
        {
            return _dbContext
                .FtpCredentials
                .Include(x => x.MasterAccounts)
                .Where(c => !c.Deleted)
                .ToList();
        }

        public ICollection<ImportedFile> ImportedFileList()
        {
            return _dbContext
                .ImportedFiles
                .Include(f => f.MasterAccount)
                .Where(c => !c.Deleted)
                .ToList();
        }

        public IEnumerable<TransitFiles> GetTransitFileList()
        {
            return _dbContext.TransitFiles.ToList();
        }

        public ImportedFile AddImportedFile(ImportedFile importedFile)
        {
            return _dbContext.ImportedFiles.Add(importedFile);
        }
    }
}