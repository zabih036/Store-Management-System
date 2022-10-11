using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Investor
    {
        public Investor()
        {
            BenifitPayment = new HashSet<BenifitPayment>();
            InvestMoney = new HashSet<InvestMoney>();
        }

        public int InvestorId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Description { get; set; }
        public int? DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<BenifitPayment> BenifitPayment { get; set; }
        public virtual ICollection<InvestMoney> InvestMoney { get; set; }
    }
}
