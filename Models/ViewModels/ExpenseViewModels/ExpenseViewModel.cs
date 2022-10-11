using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ExpenseViewModel
    {

        public long ExpenseId { get; set; }

        [Required(ErrorMessage = " مصرف شوی پیسې ولیکی ")]
        [Display(Name = " مصرف شوی پیسې :")]
        [DataType(DataType.Currency, ErrorMessage = "تاسو یوازی نمبر داخلولی شی.")]
        public double Expense { get; set; }

        [Required(ErrorMessage = " پولې واحد انتخاب کړی ")]
        [Display(Name = " پولې واحد :")]
        public int ECurrencyId { get; set; }

        [Required(ErrorMessage = " د مصرف  نوع انتخاب کړی ")]
        [Display(Name = " د مصرف نوع :")]
        [ForeignKey("ExpenseTypeId")]
        public int ExpenseTypeId { get; set; }


        [Required(ErrorMessage = "  نیټه انتخاب کړئ ")]
        [Display(Name = " نیټه :")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpenseDate { get; set; }

        [Display(Name = "تفصیل:")]
        [DataType(DataType.Text)]
        public string Detail { get; set; }
    }
}
