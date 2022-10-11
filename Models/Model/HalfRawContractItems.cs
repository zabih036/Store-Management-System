using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class HalfContractItems
    {
        public int ContractItemsId { get; set; }
        public double? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public string Desc { get; set; }
        public int? HalfContractId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual HalfContract HalfContract { get; set; }
    }
}
