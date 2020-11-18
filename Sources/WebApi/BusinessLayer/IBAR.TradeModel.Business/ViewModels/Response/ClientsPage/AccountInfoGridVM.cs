using System;
using System.Collections.Generic;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Business.ViewModels.Response.ClientsPage
{
    public class AccountInfoGridVM
    {
        public long Id { get; set; }
        public string AccountName { get; set; }
        public string Name { get; set; }
        public string СountryResidentialAddress { get; set; }
        public string CityResidentialAddress { get; set; }
        public string StreetResidentialAddress { get; set; }
        public string PostalCode { get; set; }
        public string PrimaryEmail { get; set; }
        public string Currency { get; set; }
        public string AccountCapabilities { get; set; }
        public string CustomerType { get; set; }
        public TradeRank TradeRank { get; set; }
        public string MasterAccount { get; set; }
        public DateTime? DateOpened { get; set; }
        public DateTime? DateClosed { get; set; }
        public DateTime? DateFunded { get; set; }
        public IEnumerable<TradingPermissionsGridVM> TradingPermissions { get; set; }
        public IEnumerable<TradeAccountNoteVm> TradeAccountNotes { get; set; }
    }
}