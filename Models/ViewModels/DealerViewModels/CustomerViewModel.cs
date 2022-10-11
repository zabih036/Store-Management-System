using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "مشترې نوم ولیکئ")]
        public string CusName { get; set; }


        [Required(ErrorMessage = "مشترې د پلار نوم ولیکئ")]
        public string CusFName { get; set; }


        [Required(ErrorMessage = "مشترې آدرس ولیکئ")]
        public string CusAddress { get; set; }

        [Required(ErrorMessage = "مشترې موبایل نمبر ولیکئ")]
        public string CusPhone { get; set; }

    }
}
