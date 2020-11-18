using IBAR.TradeModel.Data.Entities;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.FileSystemProviders
{
    public interface IFileManagerService
    {
        string FolderName { get; }
        void SaveFile(ImportedFile file);
        Task SaveFileAsync(ImportedFile file);
    }
}