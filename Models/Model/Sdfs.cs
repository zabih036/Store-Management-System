using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Sdfs
    {
        public DateTime? HireDate { get; set; }
        public double? Salary { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public double? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
    }
}
