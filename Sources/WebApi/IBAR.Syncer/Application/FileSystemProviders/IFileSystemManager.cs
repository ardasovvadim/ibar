using System.IO;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.Syncer.Application.FileSystemProviders
{
    public interface IFileSystemManager
    {
        void SaveFile(ImportedFile file);
    }
}