using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class SaleStockItems
    {
        public long SaleId { get; set; }
        public string BillNo { get; set; }
        public int? CustomerId { get; set; }
        public string TempCustomerName { get; set; }
        public int? ItemId { get; set; }
        public int? CurrencyId { get; set; }
        public double SalePrice { get; set; }
        public double Quantity { get; set; }
        public double? Discount { get; set; }
        public DateTime? SaleDate { get; set; }
        public int? EmployeeId { get; set; }
        public string Note { get; set; }
        public string SaleState { get; set; }
        public double PurchasePrice { get; set; }
        public int? StockId { get; set; }
        public int? UnitId { get; set; }
        public string SaleType { get; set; }
        public int AmountInUnit { get; set; }
        public string Type { get; set; }
        public long? StockItemId { get; set; }
        public int? DepartmentId { get; set; }
    }
}
