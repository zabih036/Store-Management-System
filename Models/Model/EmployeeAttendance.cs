using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class EmployeeAttendance
    {
        public int SheetId { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
