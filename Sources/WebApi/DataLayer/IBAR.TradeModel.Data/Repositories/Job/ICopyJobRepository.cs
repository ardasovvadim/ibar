using IBAR.TradeModel.Data.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface ICopyJobRepository : IBaseJobRepository
    {
        ImportedFile UpdateImportedFile(ImportedFile currentFile);
        IQueryable<ImportedFile> RegisteredFilesQuery();
        IEnumerable<ImportedFile> LoadedFailedFileList();
    }

    public class CopyJobRepository : BaseRepository, ICopyJobRepository
    {
        public IQueryable<ImportedFile> RegisteredFilesQuery()
        {
            return _dbContext
                    .ImportedFiles
                    .Include(f => f.FtpCredential)
                    .Where(f => !f.Deleted && f.FileState == FileState.Registered && f.FileStatus == FileStatus.Success);
        }
        public IEnumerable<ImportedFile> LoadedFailedFileList()
        {
            return _dbContext
                    .ImportedFiles
                    .Include(f => f.FtpCredential)
                    .Where(f => !f.Deleted && f.FileState == FileState.Loaded && f.FileStatus == FileStatus.Failed)
                    .ToList();
        }

        public ImportedFile UpdateImportedFile(ImportedFile currentFile)
        {
            _dbContext.Entry(currentFile).State = EntityState.Modified;
            return currentFile;
        }
    }
}