using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Purchase
    {
        public long PurchaseId { get; set; }
        public string BillNo { get; set; }
        public int? DealerId { get; set; }
        public int? CurrencyId { get; set; }
        public string Employee { get; set; }
        public int? ItemId { get; set; }
        public int? UnitId { get; set; }
        public int InUnit { get; set; }
        public double Quantity { get; set; }
        public double Quantity2 { get; set; }
        public double Price { get; set; }
        public double Expense { get; set; }
        public string Status { get; set; }
        public int? StockId { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string Desc { get; set; }
        public long? StockItemId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Dealer Dealer { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
