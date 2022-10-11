using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class InvestorViewModel
    {
        public int InvestorId { get; set; }
        public string defalut { get; set; }

        [Required(ErrorMessage = " د شراکتې نوم ولیکئ ")]
        [Display(Name = " شراکتې نوم :")]
        public string Name { get; set; }

        [Required(ErrorMessage = " موبایل نمبر ولیکئ ")]
        [Display(Name = " موبایل نمبر :")]
        public string Phone { get; set; }

        [Required(ErrorMessage = " بریښنالیک ولیکئ ")]
        [Display(Name = "بریښنالیک :")]
        [EmailAddress(ErrorMessage = " بریښنالیک سم نه دی")]
       // [Remote("IsInvestorEmailExist", "Investment", AdditionalFields = "Email,InvestorId", ErrorMessage = " نوموړی بریښنالیک د بل شراکتې لپاره استعمال شوی .")]
        public string Email { get; set; }


        [Required(ErrorMessage = " د راجستر کولو نیټه ولیکئ ")]
        [Display(Name = " راجستر نیټه :")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now.Date;

        [Display(Name = " تفصیلات :")]
        public string Description { get; set; }

        [Display(Name = " د شراکتې تصویر :")]
        // [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        public string oldEmail { get; set; }



    }
}
