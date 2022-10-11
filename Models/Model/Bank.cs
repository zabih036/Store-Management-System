using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Bank
    {
        public Bank()
        {
            BankDeal = new HashSet<BankDeal>();
        }

        public int BankId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<BankDeal> BankDeal { get; set; }
    }
}
