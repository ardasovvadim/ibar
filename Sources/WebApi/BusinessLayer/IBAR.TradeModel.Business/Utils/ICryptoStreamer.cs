using System;
using System.Configuration;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using PGPSnippet.Keys;
using PGPSnippet.PGPDecryption;
using PGPSnippet.PGPEncryption;

namespace IBAR.TradeModel.Business.Utils
{
    public interface ICryptoStreamer
    {
        byte[] Decrypt(byte[] encData);
        byte[] Encrypt(byte[] data);
    }

    public abstract class CryptoStreamerBase : ICryptoStreamer
    {
        private const string _passPhrase = "Cairo852";

        public byte[] Decrypt(byte[] encData)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (MemoryStream inputStream = new MemoryStream(encData))
                {
                    inputStream.Position = 0;
                    using (Stream keyStream = GetDecryptionKey())
                    {
                        PGPDecrypt.Decrypt(inputStream, keyStream, _passPhrase, outputStream);
                    }
                }

                return outputStream.ToArray();
            }
        }

        public byte[] Encrypt(byte[] data)
        {
            string tempDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            string fileName = Path.Combine(tempDir, Guid.NewGuid().ToString() + ".txt");
            using (var stream = File.Create(fileName))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }

            try
            {
                using (MemoryStream outputStream = new MemoryStream())
                {
                    PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(
                        GetPathToEncryptFile(),
                        GetPathToDecryptFile(),
                        _passPhrase);
                    PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);

                    encrypter.EncryptAndSign(outputStream, new FileInfo(fileName));

                    return outputStream.ToArray();
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        private string GetPathToEncryptFile()
        {
            return Path.Combine(ResolveKeyDirPath(), "EncryptKey.txt");
        }

        private string GetPathToDecryptFile()
        {
            return Path.Combine(ResolveKeyDirPath(), "DecryptKey.txt");
        }

        protected string ResolveKeyDirPath()
        {
            return @"C:\Temp\EncryptionKeys";
        }

        protected abstract Stream GetDecryptionKey();
    }

    public class CryptoStreamerForFileSystem : CryptoStreamerBase
    {
        protected override Stream GetDecryptionKey()
        {
            return File.Open(Path.Combine(ResolveKeyDirPath(), "DecryptKey.asc"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }

    public class CryptoStreamerForBlobStorage : CryptoStreamerBase
    {
        private readonly CloudStorageAccount _storageAccount;
        public CryptoStreamerForBlobStorage()
        {
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
        }

        protected override Stream GetDecryptionKey()
        {
            CloudBlobClient blobService = _storageAccount.CreateCloudBlobClient();
            CloudBlobContainer containerCl = blobService.GetContainerReference("decrypt");
            containerCl.CreateIfNotExists();
            var srcBlob = containerCl.GetBlockBlobReference("DecryptKey.asc");
            MemoryStream memoryStream = new MemoryStream();

            srcBlob.DownloadToStream(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}