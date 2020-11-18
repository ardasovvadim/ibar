using IBAR.Syncer.Tools.FileSystem.Ftp;
using IBAR.TradeModel.Data.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace IBAR.Syncer.Application.FileSystemProviders
{
    class AzureBlobStorageProvider : IFileSystemManager
    {
        private readonly FtpLoader _ftpLoader;
        private readonly string _historyDir = ConfigurationManager.AppSettings["historyFolder"];

        public AzureBlobStorageProvider(FtpLoader ftpLoader)
        {
            _ftpLoader = ftpLoader;
        }

        public void SaveFile(ImportedFile file)
        {
            using (var responseStream = _ftpLoader.LoadFileFromFtp(file))
            {
                CloudStorageAccount storageAccount =
                    CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
                CloudBlobClient blobServiceClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer containerClient = blobServiceClient.GetContainerReference(_historyDir);
                containerClient.CreateIfNotExists();
                var blockBlob = containerClient.GetBlockBlobReference(file.OriginalFileName);
                blockBlob.Properties.ContentType = "text/xml";

                blockBlob.UploadFromStream(responseStream);
            }
        }
    }
}