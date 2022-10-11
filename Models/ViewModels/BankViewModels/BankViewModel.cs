using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class BankViewModel
    {

        public int BankId { get; set; }

        [Display(Name = "بانک / صراف:")]
        [Required(ErrorMessage = "بانک / صراف نوم ولیکئ!!")]
        public string NameB { get; set; }

        [Display(Name = "موبایل:")]
        [Required(ErrorMessage = "مبائیل شمېره داخل کړئ!!")]
        public string PhoneB { get; set; }

        [Display(Name = "آدرس:")]
        [Required(ErrorMessage = " آدرس ولیکئ !!")]
        public string AddressB { get; set; }
    }
}
