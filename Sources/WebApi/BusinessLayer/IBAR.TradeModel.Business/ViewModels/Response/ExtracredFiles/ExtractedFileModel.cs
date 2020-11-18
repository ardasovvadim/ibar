using System;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Business.ViewModels.Response.ExtracredFiles
{
    public class SourceFileModel
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime DateOfFile { get; set; }
        public DateTime? DateOfTheProcessing { get; set; }
        public FileState FileState { get; set; }
        public FileStatus FileStatus { get; set; }
        public int Exception { get; set; }
        public bool IsForApi { get; set; }
        public bool IsSent { get; set; }
    }
}