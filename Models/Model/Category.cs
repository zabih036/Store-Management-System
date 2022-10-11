using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Category
    {
        public Category()
        {
            Item = new HashSet<Item>();
        }

        public int CategoryId { get; set; }
        public string Category1 { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Item> Item { get; set; }
    }
}
