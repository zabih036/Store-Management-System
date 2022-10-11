using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class BankDealViewModel
    {
        public long BankDealId { get; set; }

        [Required(ErrorMessage = "بانک / صراف انتخاب کړئ")]
        public int BankId { get; set; }

        [Required(ErrorMessage = "پولې واحد انتخاب کړئ")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "جمع / اخستل انتخاب کړئ")]
        public string Type { get; set; }

        [Required(ErrorMessage = "پیسو اندازه")]
        public double Amount { get; set; }

        public string Detail { get; set; }

    }
}
