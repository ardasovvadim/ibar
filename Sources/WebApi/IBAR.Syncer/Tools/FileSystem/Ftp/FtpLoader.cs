using IBAR.Syncer.Application.Helpers;
using IBAR.TradeModel.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace IBAR.Syncer.Tools.FileSystem.Ftp
{
    public class FtpLoader
    {
        private readonly FileNameMatcher _fileNameMatcher;
        
        public FtpLoader(FileNameMatcher fileNameMatcher)
        {
            _fileNameMatcher = fileNameMatcher;
        }

        public IEnumerable<(string fileName, string accountName, DateTime dateCreation)> LoadFiles(
            FtpCredential ftpCred)
        {
            var ftpUrl = ftpCred.Url;

            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(ftpUrl);
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);
            
            using (var response = (FtpWebResponse) request.GetResponse())
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

        public Stream LoadFileFromFtp(ImportedFile file)
        {
            var ftpCred = file.FtpCredential;
            
            var request = (FtpWebRequest) WebRequest.Create(ftpCred.Url + file.OriginalFileName);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpCred.UserName, ftpCred.UserPassword);
            
            var response = (FtpWebResponse) request.GetResponse();
            var responseStream = response.GetResponseStream();

            return responseStream;
        }
    }
}