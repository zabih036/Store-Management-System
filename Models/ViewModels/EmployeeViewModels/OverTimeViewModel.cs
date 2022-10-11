using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class OverTimeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "د پیسو اندازه ورسوئ!!")]
        [Display(Name = "پیسو اندازه:")]
        public double Amount { get; set; }

        [Required(ErrorMessage = "نیټه انتخاب کړئ!!")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string Details { get; set; }

    }
}
