using IBAR.TradeModel.Data.Entities;
using System.Linq;
using IBAR.TradeModel.Data.Entities.Trade;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IImportRepository
    {
        IQueryable<ImportedFile> ImportedFilesQuery();
        string GetOriginalFileName(long id);
        IQueryable<FileUpload> FileUploadQuery();
    }

    public class ImportRepository : IImportRepository
    {
        private readonly TradeModelContext _dbContext;

        public ImportRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ImportedFile> ImportedFilesQuery()
        {
            return _dbContext.ImportedFiles.Where(f => !f.Deleted);
        }

        public string GetOriginalFileName(long id)
        {
            return _dbContext
                    .ImportedFiles
                    .SingleOrDefault(f => !f.Deleted && f.Id == id)?
                    .OriginalFileName;
        }

        public IQueryable<FileUpload> FileUploadQuery()
        {
            return _dbContext.FileUploads.Where(f => !f.Deleted);
        }
    }
}