using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Dealer
    {
        public Dealer()
        {
            Purchase = new HashSet<Purchase>();
        }

        public int DealerId { get; set; }
        public string Name { get; set; }
        public string Fname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
    }
}
