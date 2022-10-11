using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class ItemViewModel
    {
        public int ItemId { get; set; }

        [Required(ErrorMessage = "د جنس نوم ولیکی ")]
        [Display(Name = "د جنس نوم:")]
        [Remote("IsItemExist", "Item", AdditionalFields = "ItemId", ErrorMessage = "نوموړی جنس شتون لرې تاسو نه شی کولی په دی نوم بل جنس ذخیره کړی.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "د جنس کټګورې انتخاب کړی ")]
        [Display(Name = "د جنس کټګورې:")]
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
    }
}
