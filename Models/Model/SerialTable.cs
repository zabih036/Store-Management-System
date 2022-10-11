using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class SerialTable
    {
        public int SerialNumberId { get; set; }
        public long PurchaseSerial { get; set; }
        public long CashSerial { get; set; }
        public long SaleSerial { get; set; }
        public long BankSerial { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
