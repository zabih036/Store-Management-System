using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class ProcessedContractItems
    {
        public int ProcessedContractItemsId { get; set; }
        public int? ProcessedContractId { get; set; }
        public double? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public string Desc { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ProcessedContract ProcessedContract { get; set; }
    }
}
