using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class ProcessedItemPurchase
    {
        public long PurchaseId { get; set; }
        public string BillNo { get; set; }
        public int? DealerId { get; set; }
        public int? CurrencyId { get; set; }
        public string Employee { get; set; }
        public int? ItemId { get; set; }
        public string Unit { get; set; }
        public int InUnit { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public int? StockId { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public long? ProcessedIronId { get; set; }
        public int? ContractId { get; set; }
        public int? DepartmentId { get; set; }
        public string Desc { get; set; }

        public virtual Dealer Dealer { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
    }
}
