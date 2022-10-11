using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Employee
    {
        public Employee()
        {
            AttendanceSheet = new HashSet<AttendanceSheet>();
            EmployeeAttendance = new HashSet<EmployeeAttendance>();
            OverTime = new HashSet<OverTime>();
            SalaryPayment = new HashSet<SalaryPayment>();
            Stock = new HashSet<Stock>();
        }

        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Address { get; set; }
        public string TazkiraNo { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? HireDate { get; set; }
        public double? Salary { get; set; }
        public int? CurrencyId { get; set; }
        public string Job { get; set; }
        public string Image { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<AttendanceSheet> AttendanceSheet { get; set; }
        public virtual ICollection<EmployeeAttendance> EmployeeAttendance { get; set; }
        public virtual ICollection<OverTime> OverTime { get; set; }
        public virtual ICollection<SalaryPayment> SalaryPayment { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
    }
}
