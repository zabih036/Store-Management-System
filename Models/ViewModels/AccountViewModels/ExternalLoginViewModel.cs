using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
