using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ItemCategoryViewModel
    {

        public int CategoryId { get; set; }

        [Required(ErrorMessage = " د کټګورې نوم ولیکی ")]
        [Display(Name = " کټګورې نوم :")]
        [DataType(DataType.Text)]
        [Remote("IsItemCategoryExist", "Item", AdditionalFields = "CategoryId", ErrorMessage = "نوموړی کټګورې شتون لرې تاسو نه شی کولی په دی نوم بله کټګورې ذخیره کړی.")]
        public string Category { get; set; }
    }
}
