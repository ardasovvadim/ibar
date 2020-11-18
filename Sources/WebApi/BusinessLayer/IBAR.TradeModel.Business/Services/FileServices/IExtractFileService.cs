using IBAR.TradeModel.Business.Utils;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.IO.Compression;

namespace IBAR.TradeModel.Business.Services.FileServices
{
    public interface IExtractFileService
    {
        Stream ExtractFile(string originalFileName);
    }

    public abstract class ExtractFileBaseService : IExtractFileService
    {
        protected string BaseDir { get; }
        protected readonly ICryptoStreamer _cryptoStreamer;

        public ExtractFileBaseService(
            ICryptoStreamer cryptoStreamer)
        {
            _cryptoStreamer = cryptoStreamer;

            BaseDir = ConfigurationManager.AppSettings["historyFolder"];
            if (string.IsNullOrEmpty(BaseDir))
                throw new ConfigurationErrorsException("Please add 'historyFolder' settigns to .config file.");

        }

        public Stream ExtractFile(string originalFileName)
        {
            return GetExtractFileStream(originalFileName, true, 0);
        }

        protected byte[] UnGzipGz(byte[] archive)
        {
            using (var compressStream = new MemoryStream(archive))
            {
                using (var compressor = new GZipStream(compressStream, CompressionMode.Decompress))
                {
                    using (var result = new MemoryStream())
                    {
                        compressor.CopyTo(result);

                        result.Position = 0;

                        return result.ToArray();
                    }
                }
            }
        }
        protected abstract Stream GetExtractFileStream(string originalFileName, bool canContinue, int i);
    }

    public class ExtractFileFromFileSystemService : ExtractFileBaseService
    {
        public ExtractFileFromFileSystemService(ICryptoStreamer cryptoStreamer) : base(cryptoStreamer) { }

        protected override Stream GetExtractFileStream(string originalFileName, bool canContinue, int i)
        {
            var originFilePath = Path.Combine(BaseDir, originalFileName);
            if (File.Exists(originFilePath))
            {
                byte[] byteFile = null;
                do
                {
                    switch (Path.GetExtension(originalFileName))
                    {
                        case ".xml":
                            byteFile = byteFile ?? File.ReadAllBytes(originFilePath);
                            canContinue = false;
                            break;
                        case ".asc":
                        case ".gpt":
                            var file = File.ReadAllBytes(originFilePath);
                            byteFile = _cryptoStreamer.Decrypt(file);
                            originalFileName = originalFileName.Replace(Path.GetExtension(originalFileName), "");
                            break;
                        case ".gz":
                            byteFile = UnGzipGz(byteFile);
                            originalFileName = originalFileName.Replace(Path.GetExtension(originalFileName), "");
                            break;
                        default:
                            canContinue = false;
                            break;
                    }
                } while (canContinue && ++i < 3);

                Stream stream = new MemoryStream(byteFile);

                return stream;
            }

            throw new FileNotFoundException("File not exist.");
        }
    }

    public class ExtractFileFromBlobStorageService : ExtractFileBaseService
    {
        private readonly CloudStorageAccount _storageAccount;
        public ExtractFileFromBlobStorageService(ICryptoStreamer cryptoStreamer) : base(cryptoStreamer)
        {
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
        }

        protected override Stream GetExtractFileStream(string originalFileName, bool canContinue, int i)
        {
            CloudBlobClient blobService = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerCl = blobService.GetContainerReference(BaseDir);
            containerCl.CreateIfNotExists();
            var srcBlob = containerCl.GetBlockBlobReference(originalFileName);
            if (srcBlob.Exists())
            {
                MemoryStream memoryStream = new MemoryStream();

                srcBlob.DownloadToStream(memoryStream);
                memoryStream.Position = 0;
                byte[] byteFile = null;

                do
                {
                    switch (Path.GetExtension(originalFileName))
                    {
                        case ".xml":
                            byteFile = byteFile ?? memoryStream.ToArray();
                            canContinue = false;
                            break;
                        case ".asc":
                        case ".gpt":
                            byteFile = base._cryptoStreamer.Decrypt(memoryStream.ToArray());
                            originalFileName = originalFileName.Replace(Path.GetExtension(originalFileName), "");
                            break;
                        case ".gz":
                            byteFile = UnGzipGz(byteFile);
                            originalFileName = originalFileName.Replace(Path.GetExtension(originalFileName), "");
                            break;
                        default:
                            canContinue = false;
                            break;
                    }
                } while (canContinue && ++i < 3);

                Stream stream = new MemoryStream(byteFile);

                return stream;
            }

            throw new ApplicationException("File not exist");
        }
    }
}