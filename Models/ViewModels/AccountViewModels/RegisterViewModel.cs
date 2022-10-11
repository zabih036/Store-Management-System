using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "د کوم کارمند لپاره چی اکاونټ جوړی هغه انتخاب کړی ")]
        [Display(Name = "کارمندانو لیست:")]
        [ForeignKey("EmployeeId")]
        [Remote("IsEmployeeAccountExist", "Account", AdditionalFields = "register_EmployeeId", ErrorMessage = " نوموړی کارمند اکاونټ موجود دی تاسو نه شی کولی بل اکاونټ جوړ کړی .")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "پټ نوم باید ولیکی")]
        [StringLength(100, ErrorMessage = " پټ نوم باید شپږو حروفو څخه کم نه وې ", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "پټ نوم")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "د پټ نوم تایدول ")]
        [Compare("Password", ErrorMessage = "د پټ نوم تایدی غلطه ده.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "د کارمند لپاره ډیپارټمنټ انتخاب کړی ")]
        [Display(Name = "ډیپارټمنټ لست:")]
        [ForeignKey("id")]
        public string id { get; set; }

        [Required(ErrorMessage = "د کارمند لپاره ډیپارټمنټ انتخاب کړی ")]
        [Display(Name = "ډیپارټمنټ لست:")]
        [ForeignKey("id2")]
        public string id2 { get; set; } = "jan";
        public string email { get; set; }
        public string email2 { get; set; }
        public string role { get; set; }
        public string trueOrfalse { get; set; }


    }
}
