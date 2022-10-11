using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class CashInHand
    {
        public long CashId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public int? CurrencyId { get; set; }
        public DateTime? Date { get; set; }
        public long? CusDealId { get; set; }
        public long? DealId { get; set; }
        public long? ExpenseId { get; set; }
        public long? BankDealId { get; set; }
        public int? SalaryId { get; set; }
        public int? InvestId { get; set; }
        public long? PrintCashId { get; set; }
        public long? CashRecordId { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
    }
}
