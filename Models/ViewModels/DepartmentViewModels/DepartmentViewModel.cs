using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class DepartmentViewModel
    {
        [Required(ErrorMessage = " د څانګې ایدې نمبر ولیکئ!!")]
        [Display(Name = "څانګې ایدې:")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = " د څانګې نوم ولیکئ!!")]
        [Display(Name = "څانګې نوم:")]
        [Remote("IsDepartmentExist", "Admin", AdditionalFields = "DepartmentId", ErrorMessage = "نوموړې څانګه شتون لري.")]
        public string Name { get; set; }

        /////////////////////////////////////////////////////////////
        [Required(ErrorMessage = "د څانګې موقعیت ولیکئ!!")]
        [Display(Name = "موقعیت:")]
        public string Location { get; set; }

        [Required(ErrorMessage = "د څانګې د مدیر بریښنالیک ولیکئ!!")]
        [Display(Name = "بریښنالیک:")]
        [DataType(DataType.EmailAddress)] 
        [Remote("IsEmployeeAccountExist", "Admin", AdditionalFields = "Email,DepartmentId", ErrorMessage = "نوموړې بریښنالیک استعمال شوی  .")]
        public string Email { get; set; }
        ///////////////////////////////////////////////////////////
        public string OldEmail { get; set; }
    }
}
