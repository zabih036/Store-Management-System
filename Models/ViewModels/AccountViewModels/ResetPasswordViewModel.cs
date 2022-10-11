using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "بریښنالیک مو ولیکئ.")]
        [EmailAddress(ErrorMessage = "بریښنالیک غلط دی.")]
        [Display(Name = "بریښنالیک")]
        public string Email { get; set; }

        [Required(ErrorMessage = "پټ نوم ولیکئ.")]
        [StringLength(100, ErrorMessage = "پټ نوم باید شپږو حروفو څخه کم نه وي.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "پټ نوم")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "تایدې پټ نوم مو سم نه دی.")]
        [Display(Name = "پټ نوم تایدې")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
