using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralSalesDb.Models.ViewModels
{
    public class LastAssetsViewModel
    {
        public int AssetId { get; set; }

        [Required(ErrorMessage = " افغانې سرمایه ولیکئ!!")]
        [Display(Name = "افغانې سرمایه:")]
        public double AfAsset { get; set; }

        [Required(ErrorMessage = " کلدارې سرمایه ولیکئ!!")]
        [Display(Name = "کلدارې سرمایه:")]
        public double PkAsset { get; set; }

        [Required(ErrorMessage = " ډالر سرمایه ولیکئ!!")]
        [Display(Name = "ډالر سرمایه:")]
        public double UsAsset { get; set; }

        [Required(ErrorMessage = "د سرمایې کال وټاکئ!!")]
        [Display(Name = "نیټه:")]
        public int Date { get; set; }
     
        public string Desc { get; set; }
    }
}
