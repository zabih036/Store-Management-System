using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class SaleViewModel
    {

        public long stockItemId { get; set; }
        public string BillNo { get; set; }

        [Required(ErrorMessage = "مشترې انتخاب کړئ!!")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "بارګیرې پیسو اندازه ولیکئ!!")]
        public double Loading { get; set; }

        [Required(ErrorMessage = "جنس انتخاب کړئ!!")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "پولې واحد انتخاب کړئ!!")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "ګودام انتخاب کړئ!!")]
        public int StockId { get; set; }

        [Required(ErrorMessage = "د خرڅ قیمت ولیکئ!!")]
        public double SalePrice { get; set; }
  
        [Required(ErrorMessage = "وزن اندازه ولیکئ !!")]
        public double Qty { get; set; }

        [Required(ErrorMessage = "واحد انتخاب کړئ !!")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "تخفیف اندازه ولیکئ !!")]
        public double Discount { get; set; }

        public double Price { get; set; }

        public string Type { get; set; }
       
    }
}
