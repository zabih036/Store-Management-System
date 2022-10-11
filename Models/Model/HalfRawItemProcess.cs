using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class HalfRawItemProcess
    {
        public long HalfRawItemProcessId { get; set; }
        public int? ItemId { get; set; }
        public double? Labor { get; set; }
        public double? Electricity { get; set; }
        public double? StockExpense { get; set; }
        public double? OtherExpenses { get; set; }
        public double? UsedHalfRawQty { get; set; }
        public double? ChemicalQty { get; set; }
        public double? ProcessedQty { get; set; }
        public double? Price { get; set; }
        public double? Wastage { get; set; }
        public DateTime? Date { get; set; }
        public long? ProcessedItemId { get; set; }
        public string Desc { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
    }
}
