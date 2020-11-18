using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IDeliveryJobRepository : IBaseJobRepository
    {
        IEnumerable<FileUpload> GetFilesToSendList();
        string GetOriginalFileName(long id);
        void UpdateFileUpload(FileUpload fileUpload);
    }

    public class DeliveryJobRepository : BaseRepository, IDeliveryJobRepository
    {
        public IEnumerable<FileUpload> GetFilesToSendList()
        {
            return _dbContext
                    .FileUploads
                    .Include(x => x.ImportedFile)
                    .Where(f => !f.Deleted &&
                                f.ImportedFile.FileState != FileState.Registered &&
                                f.ImportedFile.FileStatus == FileStatus.Success &&
                                !f.IsSent)
                    .ToList();
        }

        public string GetOriginalFileName(long id)
        {
            return _dbContext
                    .ImportedFiles
                    .FirstOrDefault(x => x.Id == id && !x.Deleted)?
                    .OriginalFileName;
        }

        public void UpdateFileUpload(FileUpload fileUpload)
        {
            _dbContext.Entry(fileUpload).State = EntityState.Modified;
        }
    }
}
