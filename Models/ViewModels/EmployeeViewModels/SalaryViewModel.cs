using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class SalaryViewModel
    {
        public int EmployeeId { get; set; }
        public int SalaryId { get; set; }
        public int CurrencyId { get; set; }
        public int Category { get; set; }

        [Required(ErrorMessage = "مهربانی له مخې د پیسو اندازه ولیکی  ")]
        public double PaidAmount { get; set; }

        [Required(ErrorMessage = "مهربانی له مخې نیټه انتخاب کړی")]
        [Display(Name = "نیټه:")]
        [DataType(DataType.Date)]
        public DateTime PaidDate { get; set; }



    }
}
