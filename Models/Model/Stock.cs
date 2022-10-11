using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Stock
    {
        public Stock()
        {
            PrintTable = new HashSet<PrintTable>();
            Purchase = new HashSet<Purchase>();
            Sale = new HashSet<Sale>();
            StockItems = new HashSet<StockItems>();
        }

        public int StockId { get; set; }
        public string Stock1 { get; set; }
        public int? Responsible { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual Employee ResponsibleNavigation { get; set; }
        public virtual ICollection<PrintTable> PrintTable { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
        public virtual ICollection<StockItems> StockItems { get; set; }
    }
}
