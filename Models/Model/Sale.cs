using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Sale
    {
        public long SaleId { get; set; }
        public string BillNo { get; set; }
        public int? CustomerId { get; set; }
        public int? ItemId { get; set; }
        public int? CurrencyId { get; set; }
        public double PurchasePrice { get; set; }
        public double SalePrice { get; set; }
        public double Quantity { get; set; }
        public double? Bundle { get; set; }
        public double Discount { get; set; }
        public DateTime? SaleDate { get; set; }
        public string Employe { get; set; }
        public string Note { get; set; }
        public string SaleState { get; set; }
        public int? UnitId { get; set; }
        public string SaleType { get; set; }
        public int InUnit { get; set; }
        public int? StockId { get; set; }
        public long? StockItemId { get; set; }
        public int? SerialNumber { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
