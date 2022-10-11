using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class StockItems
    {
        public long StockItemId { get; set; }
        public int? ItemId { get; set; }
        public int? UnitId { get; set; }
        public int? CurrencyId { get; set; }
        public double Price { get; set; }
        public int InUnit { get; set; }
        public double Quantity { get; set; }
        public int? ShortageQty { get; set; }
        public int? StockId { get; set; }
        public string Type { get; set; }
        public DateTime? Date { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
        public virtual Stock Stock { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
