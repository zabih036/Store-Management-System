using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class UsedStockItems
    {
        public long UsedStockItemId { get; set; }
        public int? ItemId { get; set; }
        public string Unit { get; set; }
        public int? CurrencyId { get; set; }
        public double Price { get; set; }
        public int InUnit { get; set; }
        public double Quantity { get; set; }
        public string Type { get; set; }
        public int? Category { get; set; }
        public long? StockItemId { get; set; }
        public DateTime? Date { get; set; }
        public string Stock { get; set; }
        public string Desc { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
    }
}
