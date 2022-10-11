using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class WastedItems
    {
        public int WastedId { get; set; }
        public int? ItemId { get; set; }
        public int? Category { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public DateTime? Date { get; set; }
        public string Desc { get; set; }
        public long? RawItemProcessId { get; set; }
        public long? HalfRawItemProcessId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
