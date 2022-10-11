using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace GeneralSalesDb.Models.Model
{
    public partial class GeneralSalesDbContext : DbContext
    {
        public IConfiguration configuration { get; set; }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Assets> Assets { get; set; }
        public virtual DbSet<AttendanceSheet> AttendanceSheet { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<BankDeal> BankDeal { get; set; }
        public virtual DbSet<BenifitPayment> BenifitPayment { get; set; }
        public virtual DbSet<CashInHand> CashInHand { get; set; }
        public virtual DbSet<CashTable> CashTable { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerDeal> CustomerDeal { get; set; }
        public virtual DbSet<Dealer> Dealer { get; set; }
        public virtual DbSet<DealerDeal> DealerDeal { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAttendance> EmployeeAttendance { get; set; }
        public virtual DbSet<Expence> Expence { get; set; }
        public virtual DbSet<ExpenceType> ExpenceType { get; set; }
        public virtual DbSet<InvestMoney> InvestMoney { get; set; }
        public virtual DbSet<Investor> Investor { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<OverTime> OverTime { get; set; }
        public virtual DbSet<PrintTable> PrintTable { get; set; }
        public virtual DbSet<PurExpense> PurExpense { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<SalaryPayment> SalaryPayment { get; set; }
        public virtual DbSet<Sale> Sale { get; set; }
        public virtual DbSet<Sdfs> Sdfs { get; set; }
        public virtual DbSet<SerialTable> SerialTable { get; set; }
        public virtual DbSet<Stock> Stock { get; set; }
        public virtual DbSet<StockItems> StockItems { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<UserLoginDetail> UserLoginDetail { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            configuration = builder.Build();
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Assets>(entity =>
            {
                entity.HasKey(e => e.AssetId);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Assets)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Assets_Department");
            });

            modelBuilder.Entity<AttendanceSheet>(entity =>
            {
                entity.HasKey(e => e.SheetId);

                entity.ToTable("Attendance_Sheet");

                entity.Property(e => e.SheetId).HasColumnName("Sheet_Id");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.AttendanceSheet)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Attendance_Sheet_Department");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.AttendanceSheet)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_Attendance_Sheet_Employee");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Bank)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Bank_Department");
            });

            modelBuilder.Entity<BankDeal>(entity =>
            {
                entity.Property(e => e.Currency2).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.DealerName).HasMaxLength(50);

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.BankDeal)
                    .HasForeignKey(d => d.BankId)
                    .HasConstraintName("FK_HDealler_HDealer");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.BankDeal)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_HDealler_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.BankDeal)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BankDeal_Department");
            });

            modelBuilder.Entity<BenifitPayment>(entity =>
            {
                entity.HasKey(e => e.BenifitId);

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.MonthDate).HasColumnType("date");

                entity.Property(e => e.PaidDate).HasColumnType("date");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.BenifitPayment)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_BenifitPayment_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.BenifitPayment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_BenifitPayment_Department");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.BenifitPayment)
                    .HasForeignKey(d => d.InvestorId)
                    .HasConstraintName("FK_BenifitPayment_Investor");
            });

            modelBuilder.Entity<CashInHand>(entity =>
            {
                entity.HasKey(e => e.CashId);

                entity.Property(e => e.CurrencyId).HasColumnName("CurrencyID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CashInHand)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_CashInHand_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CashInHand)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CashInHand_Department");
            });

            modelBuilder.Entity<CashTable>(entity =>
            {
                entity.HasKey(e => e.CashRecordId);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CashTable)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_CashTable_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CashTable)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_CashTable_Department");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.Category1)
                    .HasColumnName("Category")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Category_Department");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.CurrencyId).ValueGeneratedNever();

                entity.Property(e => e.Currency1)
                    .HasColumnName("Currency")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Fname)
                    .HasColumnName("FName")
                    .HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Customer_Department");
            });

            modelBuilder.Entity<CustomerDeal>(entity =>
            {
                entity.HasKey(e => e.DealId)
                    .HasName("PK_Deal");

                entity.HasIndex(e => e.CurrencyId)
                    .HasName("IX_Deal_CurrencyID");

                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.Image).HasMaxLength(256);

                entity.Property(e => e.InWord).HasMaxLength(50);

                entity.Property(e => e.Loan).HasMaxLength(5);

                entity.Property(e => e.Type).HasMaxLength(30);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.CustomerDeal)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_Deal_Currency");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerDeal)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CustomerDeal_Customer");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CustomerDeal)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CustomerDeal_Department");
            });

            modelBuilder.Entity<Dealer>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Fname)
                    .HasColumnName("FName")
                    .HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Dealer)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Dealer_Department");
            });

            modelBuilder.Entity<DealerDeal>(entity =>
            {
                entity.HasKey(e => e.DealId)
                    .HasName("PK_Deal2");

                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.Image).HasMaxLength(256);

                entity.Property(e => e.InWord).HasMaxLength(50);

                entity.Property(e => e.Loan).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(30);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.DealerDeal)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_DealerDeal_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DealerDeal)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_DealerDeal_Department");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Location).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Document_Department");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FatherName).HasMaxLength(50);

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.Job).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.TazkiraNo).HasMaxLength(100);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Employee_Department");
            });

            modelBuilder.Entity<EmployeeAttendance>(entity =>
            {
                entity.HasKey(e => e.SheetId);

                entity.Property(e => e.SheetId).HasColumnName("Sheet_Id");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.EmployeeId).HasColumnName("Employee_Id");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeAttendance)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EmployeeAttendance_Department");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAttendance)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeAttendance_Employee");
            });

            modelBuilder.Entity<Expence>(entity =>
            {
                entity.HasIndex(e => e.Employee)
                    .HasName("IX_Expence_EmployeeID");

                entity.HasIndex(e => e.ExpenceTypeId)
                    .HasName("IX_Expence_ExpenceTypeID");

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.ExpenceDate).HasColumnType("date");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Expence)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_Expence_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Expence)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Expence_Department");

                entity.HasOne(d => d.ExpenceType)
                    .WithMany(p => p.Expence)
                    .HasForeignKey(d => d.ExpenceTypeId)
                    .HasConstraintName("FK_Expence_ExpenceType");
            });

            modelBuilder.Entity<ExpenceType>(entity =>
            {
                entity.Property(e => e.ExpenceType1)
                    .HasColumnName("ExpenceType")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.ExpenceType)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ExpenceType_Department");
            });

            modelBuilder.Entity<InvestMoney>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.Type).HasMaxLength(60);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.InvestMoney)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_InvestMoney_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.InvestMoney)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_InvestMoney_Department");

                entity.HasOne(d => d.Investor)
                    .WithMany(p => p.InvestMoney)
                    .HasForeignKey(d => d.InvestorId)
                    .HasConstraintName("FK_InvestMoney_Investor");
            });

            modelBuilder.Entity<Investor>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.Image).HasMaxLength(256);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(16);

                entity.Property(e => e.RegistrationDate).HasColumnType("date");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Investor)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Investor_Department");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("IX_Item_CategoryID");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Item_Category");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Item_Department");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.NoteId).HasColumnName("NoteID");

                entity.Property(e => e.Note1).HasColumnName("Note");

                entity.Property(e => e.NoteStatus).HasMaxLength(10);

                entity.Property(e => e.RemindDate).HasColumnType("datetime");

                entity.Property(e => e.TargetDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasMaxLength(256);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Note)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Note_Department");
            });

            modelBuilder.Entity<OverTime>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.OverTime)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OverTime_Department");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.OverTime)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OverTime_Employee");
            });

            modelBuilder.Entity<PrintTable>(entity =>
            {
                entity.HasKey(e => e.PrintId);

                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Employe).HasMaxLength(256);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.PrintTable)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_PrintTable_Currency");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.PrintTable)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_PrintTable_PrintTable");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.PrintTable)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PrintTable_Department");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.PrintTable)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("FK_PrintTable_Stock");
            });

            modelBuilder.Entity<PurExpense>(entity =>
            {
                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("date");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Employee).HasMaxLength(256);

                entity.Property(e => e.PurchaseDate).HasColumnType("date");

                entity.Property(e => e.ReturnedDate).HasColumnType("date");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_StockItemPurchase_Currency1");

                entity.HasOne(d => d.Dealer)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.DealerId)
                    .HasConstraintName("FK_StockItemPurchase_Dealer1");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StockItemPurchase_Department1");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_StockItemPurchase_Item1");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("FK_Purchase_Stock");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_StockItemPurchase_Unit1");
            });

            modelBuilder.Entity<SalaryPayment>(entity =>
            {
                entity.HasKey(e => e.SalaryId)
                    .HasName("PK_Salary");

                entity.Property(e => e.SalaryId).HasColumnName("SalaryID");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.PaidBy).HasMaxLength(256);

                entity.Property(e => e.PaidDate).HasColumnType("date");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.SalaryPayment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SalaryPayment_Department");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.SalaryPayment)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Salary_Employee");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.Property(e => e.BillNo).HasMaxLength(50);

                entity.Property(e => e.Employe).HasMaxLength(256);

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.SaleDate).HasColumnType("date");

                entity.Property(e => e.SaleState).HasMaxLength(50);

                entity.Property(e => e.SaleType).HasMaxLength(50);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_SaleProcessedItems_Currency");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_SaleProcessedItems_Customer");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SaleProcessedItems_Department");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_SaleProcessedItems_Item");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("FK_Sale_Stock");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Sale)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_Sale_Unit");
            });

            modelBuilder.Entity<Sdfs>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("sdfs");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.HireDate).HasColumnType("date");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.PaidDate).HasColumnType("date");

                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<SerialTable>(entity =>
            {
                entity.HasKey(e => e.SerialNumberId);

                entity.Property(e => e.SerialNumberId).ValueGeneratedNever();

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.SerialTable)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_SerialTable_Department");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.Location).HasMaxLength(200);

                entity.Property(e => e.Stock1)
                    .HasColumnName("Stock")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Stock_Department");

                entity.HasOne(d => d.ResponsibleNavigation)
                    .WithMany(p => p.Stock)
                    .HasForeignKey(d => d.Responsible)
                    .HasConstraintName("FK_Stock_Employee");
            });

            modelBuilder.Entity<StockItems>(entity =>
            {
                entity.HasKey(e => e.StockItemId)
                    .HasName("PK_Item2");

                entity.HasIndex(e => e.CurrencyId)
                    .HasName("IX_Item2_CurrencyID");

                entity.HasIndex(e => e.ItemId)
                    .HasName("IX_Item2_ItemID");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Type).HasMaxLength(20);

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.StockItems)
                    .HasForeignKey(d => d.CurrencyId)
                    .HasConstraintName("FK_StockItems_Currency");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.StockItems)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StockItems_Department");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.StockItems)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_StockItems_Item");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.StockItems)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("FK_StockItems_Stock");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.StockItems)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_StockItems_Unit");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.Unit1)
                    .HasColumnName("Unit")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Unit)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Unit_Department");
            });

            modelBuilder.Entity<UserLoginDetail>(entity =>
            {
                entity.HasKey(e => e.DetailId);

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.UserLoginDetail)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserLoginDetail_Department");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
