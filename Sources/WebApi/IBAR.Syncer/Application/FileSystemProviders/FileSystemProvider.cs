using System.Configuration;
using System.IO;
using IBAR.Syncer.Application.Helpers;
using IBAR.Syncer.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.Syncer.Application.FileSystemProviders
{
    class FileSystemProvider : IFileSystemManager
    {
        private readonly FtpLoader _ftpLoader;
        private readonly string _historyDir = ConfigurationManager.AppSettings["historyFolder"];

        public FileSystemProvider(FtpLoader ftpLoader)
        {
            _ftpLoader = ftpLoader;
        }

        public void SaveFile(ImportedFile file)
        {
            var filePath = Path.Combine(_historyDir, file.OriginalFileName);

            if (!File.Exists(filePath))
            {
                DownloadFile(filePath, file);
            }
            else
            {
                File.Delete(filePath);
                DownloadFile(filePath, file);
            }
        }

        private void DownloadFile(string path, ImportedFile file)
        {
            using (var fs = File.Create(path))
            {
                using (var responseStream = _ftpLoader.LoadFileFromFtp(file))
                {
                    responseStream.CopyTo(fs);
                }

                fs.Flush();
            }
        }
    }
}