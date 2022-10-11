using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class BankDeal
    {
        public long BankDealId { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double Balance { get; set; }
        public DateTime? Date { get; set; }
        public string Detail { get; set; }
        public string Employee { get; set; }
        public int? BankId { get; set; }
        public int? CurrencyId { get; set; }
        public long? SerialNo { get; set; }
        public string DealerName { get; set; }
        public string Currency2 { get; set; }
        public double? Amount { get; set; }
        public string Type { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
    }
}
