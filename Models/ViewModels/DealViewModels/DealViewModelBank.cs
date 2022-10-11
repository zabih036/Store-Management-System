using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class DealViewModelBank
    {
        public long DealId { get; set; }
        public string Type { get; set; }

        [Required(ErrorMessage = "پیسو اندازه ولیکئ")]
        public double Debit { get; set; }

        [Required(ErrorMessage = "پیسو اندازه ولیکئ")]
        public double Credit { get; set; }
        ///////////////////////////
        [Required(ErrorMessage = " انتخاب کړئ!!")]
        public int DealerID { get; set; }
        ////////////////////
        [Required(ErrorMessage = " انتخاب کړئ!!")]
        public int CurrencyID { get; set; }


        public double Token { get; set; }

        public string Detail { get; set; }
    }
}
