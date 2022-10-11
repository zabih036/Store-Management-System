using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class CustomerDeal
    {
        public long DealId { get; set; }
        public string BillNo { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Balance { get; set; }
        public DateTime? Date { get; set; }
        public string Detail { get; set; }
        public int? CurrencyId { get; set; }
        public string Employee { get; set; }
        public int? CustomerId { get; set; }
        public string Image { get; set; }
        public string Loan { get; set; }
        public string Type { get; set; }
        public long? SerialNumber { get; set; }
        public string InWord { get; set; }
        public long? PrintId { get; set; }
        public long? StockItemId { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Currency Currency { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Department Department { get; set; }
    }
}
