using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ForgotPasswordCodeViewModel
    {
        [Required(ErrorMessage = "کوډ ولیکی.")]
        [EmailAddress]
        [Display(Name = "کوډ")]
        public int Code { get; set; }
    }
}
