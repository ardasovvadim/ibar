using IBAR.Syncer.Infrastructure.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Data.Entities;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.FileSystemProviders
{
    public class FileSystemProvider : IFileManagerService
    {
        private readonly FtpLoader _ftpLoader;
        public string FolderName { get; }

        public FileSystemProvider(FtpLoader ftpLoader)
        {
            _ftpLoader = ftpLoader;
            FolderName = ConfigurationManager.AppSettings["historyFolder"];
            if (string.IsNullOrEmpty(FolderName))
                throw new ConfigurationErrorsException("Please add 'historyFolder' settigns to .config file.");

            if (!Directory.Exists(FolderName))
            {
                Directory.CreateDirectory(FolderName);
            }
        }

        public void SaveFile(ImportedFile file)
        {
            var filePath = Path.Combine(FolderName, file.OriginalFileName);

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

        public async Task SaveFileAsync(ImportedFile file)
        {
            var filePath = Path.Combine(FolderName, file.OriginalFileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await DownloadFileAsync(filePath, file);
        }

        private async Task DownloadFileAsync(string path, ImportedFile file)
        {
            using (var fs = File.Create(path))
            {
                using (var responseStream = await _ftpLoader.LoadFileFromFtpAsync(file))
                {
                    await responseStream.CopyToAsync(fs);
                }

                await fs.FlushAsync();
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