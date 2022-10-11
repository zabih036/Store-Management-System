using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentID { get; set; }

        [Required(ErrorMessage = "د اسنادو په اړه تفصیل ولیکئ ")]
        [Display(Name = "د اسنادو تفصیل:")]
        [DataType(DataType.Text)]
        public string DocumentDetails { get; set; }

        [Required(ErrorMessage = "د اسنادو تصویر انتخاب کړئ ")]
        [Display(Name = "تصویر:")]
        [DataType(DataType.Upload)]
        public string DocumentPicture { get; set; }


    }
}
