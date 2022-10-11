using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ResetPasswordForgotViewModel
    {
        public string userEmail { get; set; }

        [Required(ErrorMessage = "نوی پټ نوم ولیکی")]
        [StringLength(100, ErrorMessage = "پټ نوم باید شپږو حروفو څخه کم نه وې.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "نوی پټ نوم")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "پټ نوم تایدې")]
        [Compare("NewPassword", ErrorMessage = "نوی پټ نوم او تایدې پټ نوم سمون نه کوې.")]
        public string ConfirmPassword { get; set; }
    }
}
