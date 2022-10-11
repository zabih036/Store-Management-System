using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class PurExpense
    {
        public long PurExpenseId { get; set; }
        public string BillNo { get; set; }
        public double? Amount { get; set; }
        public int? CurrencyId { get; set; }
        public int? DealerId { get; set; }
        public DateTime? Date { get; set; }
        public string Desc { get; set; }
    }
}
