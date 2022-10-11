using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class PurchaseViewModel
    {
        public long PurchaseId { get; set; }
        public long StockItemId { get; set; }


        [Required(ErrorMessage = "بیل نمبر ولیکئ")]
        public string BillNo { get; set; }

        [Required(ErrorMessage = "کهاتدار انتخاب کړئ")]
        public int DealerId { get; set; }

        [Required(ErrorMessage = "بیل مصرف ولیکئ")]
        public double BillExpense { get; set; }

        [Required(ErrorMessage = "جنس انتخاب کړئ")]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "واحد انتخاب کړئ")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "واحد ظرفیت ولیکئ")]
        public int InUnit { get; set; }

        [Required(ErrorMessage = "عمده تعداد ولیکئ")]
        public double Qty { get; set; }

        [Required(ErrorMessage = "پرچون تعداد ولیکئ")]
        public double Qty2 { get; set; }

        [Required(ErrorMessage = "جنس قیمت ولیکئ")]
        public double Price { get; set; }

        [Required(ErrorMessage = "پولې واحد انتخاب کړئ")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "ګودام انتخاب کړئ")]
        public int StockId { get; set; }

        public string Desc { get; set; }

        [Required(ErrorMessage = "کمبودی تعداد ولیکئ")]
        public int ShortageQty { get; set; }

    }
}
