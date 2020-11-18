using System.IO;

namespace IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles
{
    public class DownloadFileModel
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
    }
}