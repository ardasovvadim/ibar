using System.Collections.Generic;
using System.Linq;
using IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles;

namespace IBAR.TradeModel.Business.Services.FileServices
{
    // TODO: NEED RENAME THE CLASS AND INTERFACE ACCORDING FUNCTIONALITY. RENAME IT !!!    
    public interface IFileService
    {
        List<T> Paging<T>(int paginIndex, int paginSize, List<T> model) where T : class;
        List<SourceFileModel> SortingSourceFiles(string sort, List<SourceFileModel> extrModel);
    }

    public class FileService : IFileService
    {
        private readonly int zeroIndex = 0;
        private readonly int firstIndex = 1;

        public List<T> Paging<T>(int paginIndex, int paginSize, List<T> model) where T : class
        {
            return model.Skip(paginIndex * paginSize).Take(paginSize).ToList();
        }

        public List<SourceFileModel> SortingSourceFiles(string sorting, List<SourceFileModel> extrModel)
        {
            var sort = sorting.Split(';');

            var descAsc = sort[firstIndex];

            switch (sort[zeroIndex])
            {
                case "fileName":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.FileName).ToList()
                        : extrModel.OrderBy(s => s.FileName).ToList();
                    break;
                case "dateOfFile":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.DateOfFile).ToList()
                        : extrModel.OrderBy(s => s.DateOfFile).ToList();
                    break;
                case "dateOfTheProcessing":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.DateOfTheProcessing).ToList()
                        : extrModel.OrderBy(s => s.DateOfTheProcessing).ToList();
                    break;
                case "isSent":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.IsForApi).ThenByDescending(s => s.IsSent).ToList()
                        : extrModel.OrderBy(s => s.IsForApi).ThenBy(s => s.IsSent).ToList();
                    break;
                case "fileState":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.FileState).ToList()
                        : extrModel.OrderBy(s => s.FileState).ToList();
                    break;
                case "fileStatus":
                    extrModel = descAsc == "desc"
                        ? extrModel.OrderByDescending(s => s.FileStatus).ToList()
                        : extrModel.OrderBy(s => s.FileStatus).ToList();
                    break;
                default:
                    extrModel.OrderBy(s => s.Id).ToList();
                    break;
            }

            return extrModel;
        }
    }

}
