using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class InvestDealViewModel
    {
        public long DealId { get; set; }

        [Display(Name = "حساب ډول:")]
        public string Type { get; set; }

        [Required(ErrorMessage = " اخستل شوی پیسو اندازه ولیکئ!!")]
        public double Debit { get; set; }

        [Required(ErrorMessage = " د اضافه شویو پیسو اندازه ولیکئ!!")]
        public double Credit { get; set; }

        [Required(ErrorMessage = "شخص انتخاب کړئ!!")]
        [ForeignKey("DealerID")]
        public int DealerID { get; set; }


        [Required(ErrorMessage = "پیسو ډول!!")]
        [ForeignKey("CurrencyID")]
        public int CurrencyID { get; set; }

        public string Detail { get; set; }
    }
}
