using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class DealViewModel
    {


        [Required(ErrorMessage = "بیل / رسید نمبر ولیکئ")]
        public string BillNo { get; set; }

        [Required(ErrorMessage = "کهاتدار انتخاب کړئ")]
        public int DealerId { get; set; }

        [Required(ErrorMessage = "مشترې انتخاب کړئ")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "پولې واحد انتخاب کړئ")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "سیف پولې واحد انتخاب کړئ")]
        public int Currency2Id { get; set; }

        [Required(ErrorMessage = "جمع / بنام انتخاب کړئ")]
        public string Type { get; set; }

        [Required(ErrorMessage = "پیسو اندازه")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "پیسو اندازه")]
        public double Amount2 { get; set; }

        public string Detail { get; set; }


    }
}
