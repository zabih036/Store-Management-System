using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class WastedItemViewModel
    {
        public int ItemId { get; set; }
        public long Item2Id { get; set; }
        public string Item { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }

        [Required(ErrorMessage = "د ضایع اندازه ورسوئ!!")]
        [Display(Name = "اندازه:")]
        public double Quantity { get; set; }
        public double PurchasePrice { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; }
       
        [Required(ErrorMessage = "د ضایع نیټه انتخاب کړئ!!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = " تفصیل:")]   
        public string Details { get; set; }

    }
}
