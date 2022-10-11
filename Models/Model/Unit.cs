using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Unit
    {
        public Unit()
        {
            Purchase = new HashSet<Purchase>();
            Sale = new HashSet<Sale>();
            StockItems = new HashSet<StockItems>();
        }

        public int UnitId { get; set; }
        public string Unit1 { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
        public virtual ICollection<StockItems> StockItems { get; set; }
    }
}
