using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralSalesDb.Models.ViewModels
{
    public class BeltysViewModel
    {
        public int BeltyId { get; set; }

        [Required(ErrorMessage = "EnterDriverName")]
        public string DriverName { get; set; }

        [Required(ErrorMessage = "EnterFatherName")]
        public string FName { get; set; }

        [Required(ErrorMessage = "EnterCarNo")]
        public int CarNo { get; set; }


        [Required(ErrorMessage = "EnterPhone")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "EnterContractName")]
        public string ContractName { get; set; }

        [Required(ErrorMessage = "EnterMineName")]
        public string MineName { get; set; }

        [Required(ErrorMessage = "EnterFillPlace")]
        public string FillPlace { get; set; }

        [Required(ErrorMessage = "EnterEmptyPlace")]
        public string EmptyPlace { get; set; }

        [Required(ErrorMessage = "EnterItemOwner")]
        public string ItemOwner { get; set; }

        [Required(ErrorMessage = "EnterWeight")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "EnterRentPerTon")]
        public double RentPerTon { get; set; }

        [Required(ErrorMessage = "EnterTotalExpense")]
        public double TotalExpense { get; set; }

        [Required(ErrorMessage = "EnterPaid")]
        public double Paid { get; set; }

        public string Details { get; set; }

    }
}
