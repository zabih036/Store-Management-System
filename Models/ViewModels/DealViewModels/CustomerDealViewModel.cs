using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class CustomerDealViewModel
    {
        public long CustomerDealId { get; set; }

        [Required(ErrorMessage = "مشترې انتخاب کړئ")]
        [Display(Name = "مشترې:")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = " پولې واحد انتخاب کړئ")]
        [Display(Name = " پولې واحد:")]
        public int CusCurrencyId { get; set; }

        [Required(ErrorMessage = "بنام/ جمع انتخاب کړئ")]
        [Display(Name = "بنام / جمع:")]
        public string CusType { get; set; }

        [Required(ErrorMessage = "د پیسو اندازه ورسوئ")]
        [Display(Name = "د پیسو اندازه:")]
        public float CusAmount { get; set; }

        public IFormFile CusImage { get; set; }

        public string Cuscamera { get; set; }

        [Display(Name = "تفصیل:")]
        public string CusDetail { get; set; }


    }
}
