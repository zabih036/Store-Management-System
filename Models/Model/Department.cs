using System;
using System.Collections.Generic;

namespace GeneralSalesDb.Models.Model
{
    public partial class Department
    {
        public Department()
        {
            Assets = new HashSet<Assets>();
            AttendanceSheet = new HashSet<AttendanceSheet>();
            Bank = new HashSet<Bank>();
            BankDeal = new HashSet<BankDeal>();
            BenifitPayment = new HashSet<BenifitPayment>();
            CashInHand = new HashSet<CashInHand>();
            CashTable = new HashSet<CashTable>();
            Category = new HashSet<Category>();
            Customer = new HashSet<Customer>();
            CustomerDeal = new HashSet<CustomerDeal>();
            Dealer = new HashSet<Dealer>();
            DealerDeal = new HashSet<DealerDeal>();
            Document = new HashSet<Document>();
            Employee = new HashSet<Employee>();
            EmployeeAttendance = new HashSet<EmployeeAttendance>();
            Expence = new HashSet<Expence>();
            ExpenceType = new HashSet<ExpenceType>();
            InvestMoney = new HashSet<InvestMoney>();
            Investor = new HashSet<Investor>();
            Item = new HashSet<Item>();
            Note = new HashSet<Note>();
            OverTime = new HashSet<OverTime>();
            PrintTable = new HashSet<PrintTable>();
            Purchase = new HashSet<Purchase>();
            SalaryPayment = new HashSet<SalaryPayment>();
            Sale = new HashSet<Sale>();
            SerialTable = new HashSet<SerialTable>();
            Stock = new HashSet<Stock>();
            StockItems = new HashSet<StockItems>();
            Unit = new HashSet<Unit>();
            UserLoginDetail = new HashSet<UserLoginDetail>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Assets> Assets { get; set; }
        public virtual ICollection<AttendanceSheet> AttendanceSheet { get; set; }
        public virtual ICollection<Bank> Bank { get; set; }
        public virtual ICollection<BankDeal> BankDeal { get; set; }
        public virtual ICollection<BenifitPayment> BenifitPayment { get; set; }
        public virtual ICollection<CashInHand> CashInHand { get; set; }
        public virtual ICollection<CashTable> CashTable { get; set; }
        public virtual ICollection<Category> Category { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<CustomerDeal> CustomerDeal { get; set; }
        public virtual ICollection<Dealer> Dealer { get; set; }
        public virtual ICollection<DealerDeal> DealerDeal { get; set; }
        public virtual ICollection<Document> Document { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<EmployeeAttendance> EmployeeAttendance { get; set; }
        public virtual ICollection<Expence> Expence { get; set; }
        public virtual ICollection<ExpenceType> ExpenceType { get; set; }
        public virtual ICollection<InvestMoney> InvestMoney { get; set; }
        public virtual ICollection<Investor> Investor { get; set; }
        public virtual ICollection<Item> Item { get; set; }
        public virtual ICollection<Note> Note { get; set; }
        public virtual ICollection<OverTime> OverTime { get; set; }
        public virtual ICollection<PrintTable> PrintTable { get; set; }
        public virtual ICollection<Purchase> Purchase { get; set; }
        public virtual ICollection<SalaryPayment> SalaryPayment { get; set; }
        public virtual ICollection<Sale> Sale { get; set; }
        public virtual ICollection<SerialTable> SerialTable { get; set; }
        public virtual ICollection<Stock> Stock { get; set; }
        public virtual ICollection<StockItems> StockItems { get; set; }
        public virtual ICollection<Unit> Unit { get; set; }
        public virtual ICollection<UserLoginDetail> UserLoginDetail { get; set; }
    }
}
