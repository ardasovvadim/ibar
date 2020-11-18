using System;

namespace IBAR.TradeModel.Business.ViewModels.Response.ClientsPage
{
    public class TradeAccountNoteVm
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public int NoteType { get; set; }
        public long TradeAccountId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}