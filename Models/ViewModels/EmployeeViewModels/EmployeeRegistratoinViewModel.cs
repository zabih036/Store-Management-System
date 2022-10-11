using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class EmployeeRegistratoinViewModel
    {
        public int EmployeeId { get; set; }
        public string defalut { get; set; }

        [Required(ErrorMessage = " د کارمند نوم ولیکی ")]
        [Display(Name = " د کارمند نوم :")]
        public string Name { get; set; }

        [Required(ErrorMessage = "  کارمند د پلار نوم ولیکی ")]
        [Display(Name = " د پلار نوم :")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "  آدرس  ولیکی ")]
        [Display(Name = " آدرس  :")]
        public string Address { get; set; }

        [Required(ErrorMessage = "  تذکیرې نمبر ولیکی ")]
        [Display(Name = " تذکیرې نمبر :")]
        public string TazkiraNo { get; set; }

        [Required(ErrorMessage = " موبایل نمبر ولیکی ")]
        [Display(Name = " موبایل نمبر :")]
        public string Phone { get; set; }

        [Required(ErrorMessage = " معاش پولې واحد انتخاب کړئ ")]
        [Display(Name = " معاش پولې واحد :")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = " بریښنالیک ولیکی ")]
        [Display(Name = "بریښنالیک :")]
        [EmailAddress(ErrorMessage = " بریښنالیک سم نه دی")]
        [Remote("IsEmployeeEmailExist", "Employee", AdditionalFields = "Email,EmployeeId", ErrorMessage = " نوموړی بریښنالیک د بل کارمند لپاره استعمال شوی .")]
        public string Email { get; set; }


        [Required(ErrorMessage = " د ګومارلو نیټه ولیکی ")]
        [Display(Name = " ګومارلو نیټه :")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; } = DateTime.Now.Date;

        [Required(ErrorMessage = " د کارمند معاش ولیکی  ")]
        [Display(Name = " د معاش اندازه :")]
        public double Salary { get; set; }

        [Required(ErrorMessage = " کارمند دنده ولیکی ")]
        [Display(Name = " کارمند دنده :")]
        [DataType(DataType.Text)]
        public string Job { get; set; }

        [Display(Name = " د کارمند تصویر :")]
        // [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        public string oldEmail { get; set; }



    }
}
