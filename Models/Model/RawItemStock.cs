using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class RawItemStock
    {
        public long RawItemId { get; set; }
        public int ItemId { get; set; }
        public string Unit { get; set; }
        public int? InUnit { get; set; }
        public int? CurrencyId { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public int? StockId { get; set; }
        public DateTime? Date { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
    }
}
