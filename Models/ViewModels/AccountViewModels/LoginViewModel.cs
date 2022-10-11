using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = " بریښنالیک ولیکی  ")]
        [Display(Name = " بریښنالیک :")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = " پټ نوم ولیکی ")]
        [Display(Name = " پټ نوم:")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = " په يادساتل")]
        public bool RememberMe { get; set; }
    }
}
