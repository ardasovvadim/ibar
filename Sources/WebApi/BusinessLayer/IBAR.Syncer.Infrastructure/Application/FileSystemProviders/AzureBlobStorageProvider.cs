using IBAR.Syncer.Infrastructure.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Data.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.FileSystemProviders
{
    public class AzureBlobStorageProvider : IFileManagerService
    {
        private readonly FtpLoader _ftpLoader;
        public string FolderName { get; }

        public AzureBlobStorageProvider(FtpLoader ftpLoader)
        {
            _ftpLoader = ftpLoader;
            FolderName = ConfigurationManager.AppSettings["historyFolder"];
            if (string.IsNullOrEmpty(FolderName))
                throw new ConfigurationErrorsException("Please add 'historyFolder' settigns to .config file.");
        }

        public void SaveFile(ImportedFile file)
        {
            using (var responseStream = _ftpLoader.LoadFileFromFtp(file))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
                CloudBlobClient blobServiceClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer containerClient = blobServiceClient.GetContainerReference(FolderName);
                containerClient.CreateIfNotExists();
                var blockBlob = containerClient.GetBlockBlobReference(file.OriginalFileName);
                blockBlob.Properties.ContentType = "text/xml";

                blockBlob.UploadFromStream(responseStream);
            }
        }

        public async Task SaveFileAsync(ImportedFile file)
        {
            using (var responseStream = await _ftpLoader.LoadFileFromFtpAsync(file))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
                CloudBlobClient blobServiceClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer containerClient = blobServiceClient.GetContainerReference(FolderName);
                await containerClient.CreateIfNotExistsAsync();
                var blockBlob = containerClient.GetBlockBlobReference(file.OriginalFileName);
                blockBlob.Properties.ContentType = "text/xml";

                await blockBlob.UploadFromStreamAsync(responseStream);
            }
        }
    }
}