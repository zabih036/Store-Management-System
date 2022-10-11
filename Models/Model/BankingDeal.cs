﻿using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class BankingDeal
    {
        public long BankingDealId { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public DateTime? Date { get; set; }
        public string Detail { get; set; }
        public string Employee { get; set; }
        public int? BankingId { get; set; }
        public int? CurrencyId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Department Department { get; set; }
    }
}
