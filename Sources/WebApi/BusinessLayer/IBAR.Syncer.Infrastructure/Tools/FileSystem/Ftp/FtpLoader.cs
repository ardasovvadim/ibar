using IBAR.Syncer.Infrastructure.Application.Helpers;
using IBAR.TradeModel.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Tools.FileSystem.Ftp
{
    public class FtpLoader
    {
        private readonly FileNameMatcher _fileNameMatcher;

        public FtpLoader(FileNameMatcher fileNameMatcher)
        {
            _fileNameMatcher = fileNameMatcher;
        }

        public IEnumerable<(string fileName, string accountName, DateTime dateCreation)> LoadFiles(FtpCredential ftpCred)
        {
            var ftpUrl = ftpCred.Url;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        var fileName = "";
                        do
                        {
                            fileName = reader.ReadLine();

                            if (_fileNameMatcher.IsMatch(fileName))
                            {
                                var account = _fileNameMatcher.ParseAccountName(fileName);

                                DateTime? creationDate = null;
                                if (_fileNameMatcher.IsTradeAsReport(fileName))
                                    creationDate = DateHelper.ParseDate(fileName, 1);
                                else
                                    creationDate = DateHelper.ParseDate(fileName);

                                yield return (fileName, account, creationDate.Value);
                            }
                        } while (!string.IsNullOrEmpty(fileName));
                    }
                }
            }
        }

        public async Task<IEnumerable<(string fileName, string accountName, DateTime creationDate)>> LoadFilesAsync(FtpCredential ftpCred)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpCred.Url);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    var resultList = new HashSet<(string, string, DateTime)>();

                    using (var reader = new StreamReader(responseStream))
                    {
                        var fileName = "";
                        do
                        {
                            fileName = reader.ReadLine();

                            if (_fileNameMatcher.IsMatch(fileName))
                            {
                                var account = _fileNameMatcher.ParseAccountName(fileName);

                                DateTime? creationDate = null;
                                if (_fileNameMatcher.IsTradeAsReport(fileName))
                                    creationDate = DateHelper.ParseDate(fileName, 1);
                                else
                                    creationDate = DateHelper.ParseDate(fileName);

                                resultList.Add((fileName, account, creationDate.Value));
                            }
                        } while (!string.IsNullOrEmpty(fileName));

                        return resultList.ToList();
                    }
                }
            }
        }

        public Stream LoadFileFromFtp(ImportedFile file)
        {
            var ftpCred = file.FtpCredential;

            var request = (FtpWebRequest)WebRequest.Create(ftpCred.Url + file.OriginalFileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);

            var response = (FtpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();

            return responseStream;
        }

        public async Task<Stream> LoadFileFromFtpAsync(ImportedFile file)
        {
            var ftpCred = file.FtpCredential;

            var request = (FtpWebRequest)WebRequest.Create($"{ftpCred.Url}{file.OriginalFileName}");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);

            var response = (FtpWebResponse)await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();

            return responseStream;
        }
    }
}