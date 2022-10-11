using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class UseItemsViewModel
    {

        [Required(ErrorMessage = "استعمال بخش انتخاب کړئ")]
        public int Category { get; set; }

        [Required(ErrorMessage = "استعمال وزن / تعداد ولیکئ")]
        public double Qty { get; set; }

        [Required(ErrorMessage = "نیټه انتخاب کړئ")]
        public DateTime Date { get; set; }

        public string Details { get; set; }
    }
}
