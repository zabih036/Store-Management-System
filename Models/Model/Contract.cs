using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Contract
    {
        public Contract()
        {
            ContractItems = new HashSet<ContractItems>();
        }

        public int ContractId { get; set; }
        public string Name { get; set; }
        public int? DealerId { get; set; }
        public int? CustomerId { get; set; }
        public int ItemId { get; set; }
        public int? CurrencyId { get; set; }
        public int? InUnit { get; set; }
        public string Unit { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public int Days { get; set; }
        public string Finished { get; set; }
        public int? DepartmentId { get; set; }
        public string ContractType { get; set; }
        public string Desc { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Item Item { get; set; }
        public virtual ICollection<ContractItems> ContractItems { get; set; }
    }
}
