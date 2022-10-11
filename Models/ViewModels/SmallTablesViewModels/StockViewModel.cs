using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class StockViewModel
    {
        public int StockId { get; set; }

        [Required(ErrorMessage = "د ګودام نوم ولیکی ")]
        [Display(Name = "د ګودام نوم:")]
        [DataType(DataType.Text)]
        [Remote("IsStockExist", "ManageSmallTables", AdditionalFields = "StockId", ErrorMessage = " ګودام په دې نوم شتون لرې تاسو نه شی کولی په دی نوم بل ګودام ذخیره کړی.")]
        public string Stock { get; set; }

        [Required(ErrorMessage = "د ګودام مسول انتخاب کړئ ")]
        [Display(Name = "مسوُل:")]
        [ForeignKey("EmployeeId")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "د ګودام موقعیت ولیکی ")]
        [Display(Name = "موقعیت:")]
        public string Location { get; set; }
        [Display(Name = "تفصیل:")]
        public string Details { get; set; }
    }
}
