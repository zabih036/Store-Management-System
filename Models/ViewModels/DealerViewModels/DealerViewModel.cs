using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class DealerViewModel
    {
        public int DealerId { get; set; }

        [Required(ErrorMessage = "کهاتدار نوم ولیکئ")]
        public string Name { get; set; }


        [Required(ErrorMessage = "کهاتدار دپلار نوم ولیکئ")]
        public string FName { get; set; }


        [Required(ErrorMessage = "کهاتدار آدرس ولیکئ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "کهاتدار موبایل نمبر ولیکئ")]
        public string Phone { get; set; }

    }
}
