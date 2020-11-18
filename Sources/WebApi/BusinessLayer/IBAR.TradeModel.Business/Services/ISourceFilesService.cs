using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Business.ViewModels.Request;
using IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles;
using IBAR.TradeModel.Data.Repositories;

namespace IBAR.TradeModel.Business.Services
{
    public interface ISourcesFilesService
    {
        PaginationModel<SourceFileModel> GetSourceFilesList(int paginIndex, int paginSize, string sorting,
            Period period,
            string searchName);

        HttpResponseMessage DownloadSourceFile(long id);
    }

    public class SourceFileService : ISourcesFilesService
    {
        private readonly IImportRepository _importRepository;
        private readonly IFileService _fileService;
        private readonly IExtractFileService _extractService;

        public SourceFileService(IImportRepository importRepository, IFileService fileService,
            IExtractFileService extractService)
        {
            _importRepository = importRepository;
            _fileService = fileService;
            _extractService = extractService;
        }

        public PaginationModel<SourceFileModel> GetSourceFilesList(int paginIndex, int paginSize, string sorting,
            Period period, string searchName)
        {
            var result = new PaginationModel<SourceFileModel>();
            var filesQuery = _importRepository
                                .ImportedFilesQuery()
                                .Include(f => f.FileUpload);

            if (!string.IsNullOrEmpty(searchName))
            {
                filesQuery = filesQuery.Where(s => s.OriginalFileName.ToLower().Contains(searchName.ToLower()));
            }

            if (period != null)
            {
                filesQuery = filesQuery.Where(s =>
                    s.FileCreateDate >= period.FromDate && s.FileCreateDate <= period.ToDate);
            }

            var fileList = filesQuery.Select(f => new SourceFileModel
            {
                Id = f.Id,
                FileName = f.OriginalFileName,
                DateOfFile = f.FileCreateDate,
                DateOfTheProcessing = f.ModifiedDate,
                FileState = f.FileState,
                FileStatus = f.FileStatus,
                IsForApi = f.FileUpload != null,
                IsSent = f.FileUpload != null && f.FileUpload.IsSent
            }).ToList();

            fileList.ForEach(f =>
            {
                var index = f.FileName.IndexOf(".xml", StringComparison.Ordinal);
                f.FileName = f.FileName.Substring(0, index);
            });

            result.DataLength = fileList.Count;
            result.Data = fileList;

            if (sorting.Any())
            {
                result.Data = _fileService.SortingSourceFiles(sorting, result.Data);
            }

            result.Data = _fileService.Paging<SourceFileModel>(paginIndex, paginSize, result.Data);

            return result;
        }

        public HttpResponseMessage DownloadSourceFile(long id)
        {
            var originalFileName = _importRepository.GetOriginalFileName(id);

            var fileStream = _extractService.ExtractFile(originalFileName);

            {
                var canContinue = true;
                var i = 0;
                do
                {
                    switch (Path.GetExtension(originalFileName))
                    {
                        case ".xml":
                            canContinue = false;
                            break;
                        case ".asc":
                        case ".gpt":
                        case ".gz":
                            originalFileName = originalFileName.Replace(Path.GetExtension(originalFileName), "");
                            break;
                        default:
                            canContinue = false;
                            break;
                    }
                } while (canContinue && ++i < 3);
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(fileStream) };
            result.Headers.Add("FileName", originalFileName);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");

            return result;
        }
    }
}