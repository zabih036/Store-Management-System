using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class PurchaseExpense
    {
        public int PurchaseExpenseId { get; set; }
        public string BillNo { get; set; }
        public string OrderNo { get; set; }
        public double? Way1 { get; set; }
        public double? Way2 { get; set; }
        public double? Way3 { get; set; }
        public double? Gumrak { get; set; }
        public string CarInfo { get; set; }
        public double? Khabar { get; set; }
        public double? Khak { get; set; }
        public double? Comission { get; set; }
        public DateTime? Date { get; set; }
        public double? TotalItems { get; set; }
        public double? TotalPurchase { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
    }
}
