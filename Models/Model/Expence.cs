using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Expence
    {
        public long ExpenceId { get; set; }
        public string Employee { get; set; }
        public double ExpenceAmount { get; set; }
        public int? CurrencyId { get; set; }
        public DateTime? ExpenceDate { get; set; }
        public int? ExpenceTypeId { get; set; }
        public string Detail { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual ExpenceType ExpenceType { get; set; }
    }
}
