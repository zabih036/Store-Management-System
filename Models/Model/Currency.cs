using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Currency
    {
        public Currency()
        {
            BankDeal = new HashSet<BankDeal>();
            BenifitPayment = new HashSet<BenifitPayment>();
            CashInHand = new HashSet<CashInHand>();
            CashTable = new HashSet<CashTable>();
            CustomerDeal = new HashSet<CustomerDeal>();
            DealerDeal = new HashSet<DealerDeal>();
            Expence = new HashSet<Expence>();
            InvestMoney = new HashSet<InvestMoney>();
            PrintTable = new HashSet<PrintTable>();
            Purchase = new HashSet<Purchase>();
            Sale = new HashSet<Sale>();
            StockItems = new HashSet<StockItems>();
        }

        public int CurrencyId { get; set; }
        public string Currency1 { get; set; }

        public virtual ICollection<BankDeal> BankDeal { get; set; }
        public virtual ICollection<BenifitPayment> BenifitPayment { get; set; }
        public virtual ICollection<CashInHand> CashInHand { get; set; }
        public virtual ICollection<CashTable> CashTable { get; set; }
        public virtual ICollection<CustomerDeal> CustomerDeal { get; set; }
        public virtual ICollection<DealerDeal> DealerDeal { get; set; }
        public virtual ICollection<Expence> Expence { get; set; }
        public virtual ICollection<InvestMoney> InvestMoney { get; set; }
        public virtual ICollection<PrintTable> PrintTable { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
        public virtual ICollection<StockItems> StockItems { get; set; }
    }
}
