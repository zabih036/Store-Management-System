using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class BenifitViewModel
    {
        public int BenifitId { get; set; }
        public int InvestorId { get; set; }
        public int CurrencyId { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }

        [Display(Name = " ټوټل پیسې:")]
        [DataType(DataType.Currency)]
        public double TotalAmount { get; set; }

        [Required(ErrorMessage = "مهربانی له مخې پیسو اندازه ولیکئ ")]
        [Display(Name = "پیسو اندازه:")]
        public double Amount { get; set; }
        //public DateTime MonthDate { get; set; }
        [Display(Name = "  تفصیلات:")]
        public string Description { get; set; }



    }
}
