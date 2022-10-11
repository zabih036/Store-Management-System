using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class InvestMoney
    {
        public int InvestMoneyId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public DateTime? Date { get; set; }
        public int? InvestorId { get; set; }
        public int? CurrencyId { get; set; }
        public string Type { get; set; }
        public long? UsedStockItemId { get; set; }
        public string Description { get; set; }
        public string Employee { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
        public virtual Investor Investor { get; set; }
    }
}
