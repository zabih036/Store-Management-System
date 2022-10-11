using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class ContractItems
    {
        public int ContractItemsId { get; set; }
        public int ContractId { get; set; }
        public string Unit { get; set; }
        public int? InUnit { get; set; }
        public double? Quantity { get; set; }
        public DateTime? Date { get; set; }
        public string Desc { get; set; }
        public long? PurchaseId { get; set; }
        public long? RawPurchaseId { get; set; }
        public long? HalfRawPurchaseId { get; set; }
        public long? ProcessPurchaseId { get; set; }
        public long? SaleId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Department Department { get; set; }
    }
}
