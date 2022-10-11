using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "خپل بریښنالیک ولیکی.")]
        [EmailAddress(ErrorMessage = "بریښنالیک سم نه دی!!!")]
        [Display(Name = "بریښنالیک")]
        public string Email { get; set; }
    }
}
