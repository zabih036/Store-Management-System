
using GeneralSalesDb.Models.AccountViewModels;

namespace GeneralSalesDb.Models.ViewModels
{
    public class AllViewModel
    {
        public StockViewModel stock { get; set; }
        public ExpenseViewModel expense { get; set; }
        public ExpenseTypeViewModel expenseType { get; set; }
        public ItemViewModel item { get; set; }
        public ItemCategoryViewModel itemCategory { get; set; }
        public EmployeeRegistratoinViewModel employee { get; set; }
        public SalaryViewModel salary { get; set; }
        public RegisterViewModel register { get; set; }
        public ClaimViewModel claim { get; set; }
        public UnitViewModel unit { get; set; }
        public ForgotPasswordViewModel forgot { get; set; }
        public ForgotPasswordCodeViewModel code { get; set; }
        public ResetPasswordForgotViewModel reset { get; set; }
        public ManulReprotViewModel manulReport { get; set; }
        public InvestorViewModel investor { get; set; }
        public BenifitViewModel Benifit { get; set; }

        public OverTimeViewModel overTime { get; set; }
    }
}
