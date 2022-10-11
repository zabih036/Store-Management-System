using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class OverTime
    {
        public int OverTimeId { get; set; }
        public double OverTimeHours { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
