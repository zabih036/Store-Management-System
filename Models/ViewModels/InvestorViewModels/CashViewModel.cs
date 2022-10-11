using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class CashViewModel
    {
        public long CashId { get; set; }

        [Required(ErrorMessage = " پیسو اندازه ولیکئ!!")]
        [Display(Name = "پیسو اندازه:")]
        public double Amount { get; set; }

        [Required(ErrorMessage = " جمع / اخستل انتخاب کړئ!!")]
        [Display(Name = "جمع / اخستل:")]
        public string Type { get; set; }

        [Required(ErrorMessage = " پولې واحد انتخاب کړئ!!")]
        [Display(Name = "پولې واحد:")]
        public int CurrencyId { get; set; }

        public string Desc { get; set; }
    }
}
