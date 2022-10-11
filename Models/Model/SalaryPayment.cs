using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class SalaryPayment
    {
        public int SalaryId { get; set; }
        public double PaidAmount { get; set; }
        public int? CurrencyId { get; set; }
        public int? Category { get; set; }
        public DateTime? PaidDate { get; set; }
        public int? EmployeeId { get; set; }
        public string PaidBy { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
