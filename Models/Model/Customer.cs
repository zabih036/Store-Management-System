using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerDeal = new HashSet<CustomerDeal>();
            PrintTable = new HashSet<PrintTable>();
            Sale = new HashSet<Sale>();
        }

        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Fname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<CustomerDeal> CustomerDeal { get; set; }
        public virtual ICollection<PrintTable> PrintTable { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
    }
}
