using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ReturnItemsViewModel
    {
        public long PurchaseId { get; set; }
        public long Item2Id { get; set; }
        public int DealerId { get; set; }

        [Required(ErrorMessage = "واپسی تعداد ولیکئ")]
        public double ReturnQty { get; set; }

        public string Details { get; set; }
    }
}
