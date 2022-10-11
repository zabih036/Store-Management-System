using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ExpenseTypeViewModel
    {
        public int ExpenseTypeId { get; set; }

        [Required(ErrorMessage = " د مصرف نوع ولیکی ")]
        [Display(Name = " مصرف نوع :")]
        [DataType(DataType.Text)]
        [Remote("IsExpenseTypeExist", "Expense", AdditionalFields = "ExpenseTypeId", ErrorMessage = "د مصرف نوع شتون لرې تاسو نه شی کولی په دی نوم بل ذخیره کړی.")]

        public string ExpenseType { get; set; }
    }
}
