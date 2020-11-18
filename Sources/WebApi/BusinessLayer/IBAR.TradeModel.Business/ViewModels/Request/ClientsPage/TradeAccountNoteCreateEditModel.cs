namespace IBAR.TradeModel.Business.ViewModels.Request.ClientsPage
{
    public class TradeAccountNoteCreateEditModel
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public int NoteType { get; set; }
        public long TradeAccountId { get; set; }
    }
}