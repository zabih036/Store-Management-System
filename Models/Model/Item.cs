using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Item
    {
        public Item()
        {
            Purchase = new HashSet<Purchase>();
            Sale = new HashSet<Sale>();
            StockItems = new HashSet<StockItems>();
        }

        public int ItemId { get; set; }
        public string Name { get; set; }
        public int? CategoryId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
        public virtual ICollection<StockItems> StockItems { get; set; }
    }
}
