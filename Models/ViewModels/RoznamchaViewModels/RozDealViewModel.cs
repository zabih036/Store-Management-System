using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class RozDealViewModel
    {
        public long DealersId { get; set; }

        [Required(ErrorMessage = "کهاتدار انتخاب کړئ")]
        public int DealerId { get; set; }

        [Required(ErrorMessage = "مشترې انتخاب کړئ")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "پولې واحد انتخاب کړئ")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "قرضه / تادیه")]
        public string Type { get; set; }

        [Required(ErrorMessage = "پیسو اندازه ولیکئ")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "سیف پیسو اندازه ولیکئ")]
        public double CashAmount { get; set; }


        [Required(ErrorMessage = "سیف انتخاب کړئ")]
        public int CashId { get; set; }

        public string Detail { get; set; }


    }
}
