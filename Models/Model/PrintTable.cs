using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class PrintTable
    {
        public long PrintId { get; set; }
        public string BillNo { get; set; }
        public int? CustomerId { get; set; }
        public int? CurrencyId { get; set; }
        public int? StockId { get; set; }
        public double? TotalBill { get; set; }
        public double? Loading { get; set; }
        public double? Previous { get; set; }
        public double? Paid { get; set; }
        public DateTime? Date { get; set; }
        public string Employe { get; set; }
        public string Desc { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Department Department { get; set; }
        public virtual Stock Stock { get; set; }
    }
}
