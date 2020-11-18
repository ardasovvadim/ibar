using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles
{
    public class FtpFileModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime DateDownloaded { get; set; }

    }
}
