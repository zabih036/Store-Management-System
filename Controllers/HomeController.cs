using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralSalesDb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;

namespace GeneralSalesDb.Controllers
{

    public class HomeController : Controller
    {

        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        int depId = 0;
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public IActionResult Index()
        {

            return View();
        }

        [Authorize(Roles = ("Admin Department,Finance Department"))]
        public IActionResult dashboard()
        {
            return View();
        }

        [Authorize(Roles = ("Admin Department,Finance Department"))]
        public IActionResult MonthlyExpenses(int currencyId)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var monthStart = DateTime.Now;
            var monthEnd = DateTime.Now;
            Dictionary<string, double> dictWeeklySum = new Dictionary<string, double>();
            var currentMonthNo = DateTime.Now.Month;
            var currentYearNo = DateTime.Now.Year;
            for (int i = 1; i < 13; i++)
            {
                var last = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(-currentMonthNo + i).Month);
                monthStart = DateTime.Now.AddMonths((-currentMonthNo) + i).AddDays(-(DateTime.Now.Day - 1));
                monthEnd = DateTime.Now.AddMonths((-currentMonthNo) + i).AddDays(-(DateTime.Now.Day)).AddDays(last);


                double expense = db.Expence.Where
              (cat => cat.ExpenceDate >= monthStart && cat.ExpenceDate <= monthEnd  && cat.CurrencyId==currencyId && cat.DepartmentId == depId)
              .Select(cat => cat.ExpenceAmount)
              .Sum();

                double salaries = db.SalaryPayment.Where
              (cat => cat.PaidDate >= monthStart && cat.PaidDate <= monthEnd  && cat.CurrencyId == currencyId && cat.DepartmentId == depId)
              .Select(cat => cat.PaidAmount)
              .Sum();



                double amount = expense + salaries ;

                if (i == 1)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("جنورې", amount);
                }
                else if (i == 2)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("فیبرورې", amount);
                }
                else if (i == 3)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("مارچ", amount);
                }
                else if (i == 4)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("اپریل", amount);
                }
                else if (i == 5)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("مای", amount);
                }
                else if (i == 6)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("جون", amount);
                }
                else if (i == 7)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("جولای", amount);
                }
                else if (i == 8)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("اګست", amount);
                }
                else if (i == 9)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("سپتمبر", amount);
                }
                else if (i == 10)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("اکتوبر", amount);
                }
                else if (i == 11)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("نومبر", amount);
                }
                else if (i == 12)
                {
                    if (amount != 0)
                        dictWeeklySum.Add("ډیسمبر", amount);
                }


            }
            return new JsonResult(dictWeeklySum);
        }

        [Authorize(Roles = ("Admin Department,Finance Department"))]
        public IActionResult GeneralReport()
        {
            return View();
        }

        [Authorize(Roles = ("Admin Department,Finance Department"))]
        public JsonResult MonthlyIncomes()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            Dictionary<string, double> af = new Dictionary<string, double>();
            Dictionary<string, double> pk = new Dictionary<string, double>();
            Dictionary<string, double> us = new Dictionary<string, double>();

            for (int i = 1; i < 13; i++)
            {
                double afIncome = 0.0;
                double pkIncome = 0.0;
                double usIncome = 0.0;

                var itemsale = db.Sale.Where(r => r.SaleDate.Value.Year == DateTime.Now.Year && r.SaleDate.Value.Month == i && r.SaleState != "Returned" && r.DepartmentId == depId).ToList();

                foreach (var item in itemsale)
                {
                    if (item.CurrencyId == 1)
                    {
                        afIncome +=  ((item.Quantity * item.SalePrice) - item.Discount) - (item.Quantity * item.PurchasePrice) ;
                    }
                    else if (item.CurrencyId == 2)
                    {
                        pkIncome += ((item.Quantity * item.SalePrice) - item.Discount) - (item.Quantity * item.PurchasePrice) ;
                    }
                    else if (item.CurrencyId == 3)
                    {
                        usIncome += ((item.Quantity * item.SalePrice) - item.Discount) - (item.Quantity * item.PurchasePrice);
                    }
                }


                if (i == 1)
                {
                    af.Add("جنورې", afIncome);
                    pk.Add("جنورې", pkIncome);
                    us.Add("جنورې", usIncome);
                }
                else if (i == 2)
                {
                    af.Add("فیبرورې", afIncome);
                    pk.Add("فیبرورې", pkIncome);
                    us.Add("فیبرورې", usIncome);
                }
                else if (i == 3)
                {
                    af.Add("مارچ", afIncome);
                    pk.Add("مارچ", pkIncome);
                    us.Add("مارچ", usIncome);
                }
                else if (i == 4)
                {
                    af.Add("اپریل", afIncome);
                    pk.Add("اپریل", pkIncome);
                    us.Add("اپریل", usIncome);
                }
                else if (i == 5)
                {
                    af.Add("مای", afIncome);
                    pk.Add("مای", pkIncome);
                    us.Add("مای", usIncome);
                }
                else if (i == 6)
                {
                    af.Add("جون", afIncome);
                    pk.Add("جون", pkIncome);
                    us.Add("جون", usIncome);
                }
                else if (i == 7)
                {
                    af.Add("جولای", afIncome);
                    pk.Add("جولای", pkIncome);
                    us.Add("جولای", usIncome);
                }
                else if (i == 8)
                {
                    af.Add("اګست", afIncome);
                    pk.Add("اګست", pkIncome);
                    us.Add("اګست", usIncome);
                }
                else if (i == 9)
                {
                    af.Add("سپتمبر", afIncome);
                    pk.Add("سپتمبر", pkIncome);
                    us.Add("سپتمبر", usIncome);
                }
                else if (i == 10)
                {
                    af.Add("اکتوبر", afIncome);
                    pk.Add("اکتوبر", pkIncome);
                    us.Add("اکتوبر", usIncome);
                }
                else if (i == 11)
                {
                    af.Add("نومبر", afIncome);
                    pk.Add("نومبر", pkIncome);
                    us.Add("نومبر", usIncome);
                }
                else if (i == 12)
                {
                    af.Add("ډیسمبر", afIncome);
                    pk.Add("ډیسمبر", pkIncome);
                    us.Add("ډیسمبر", usIncome);
                }


            }

            var allCurrency = new
            {
                afghani = af,
                pk = pk,
                dollar = us,
            };
            return new JsonResult(allCurrency);
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Benefit()
        {
            return View();
        }


        public JsonResult FetchFinanceReport(int reportType)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var sale = db.Sale.Where(x => x.SaleId > 0 && x.SaleState != "Returned" && x.DepartmentId == depId);

            switch (reportType)
            {
                ////////////////////// todays report ///////////////
                case 1:

                    sale = db.Sale.Where(x => x.SaleDate == DateTime.Now.Date && x.SaleState != "Returned" && x.DepartmentId == depId);

                    break;
                /////////////////// current month's report ///////////
                case 2:
                    var currentMonth = DateTime.Now.Month;
                    sale = db.Sale.Where(x => x.SaleDate.Value.Month == currentMonth && x.SaleDate.Value.Year == DateTime.Now.Year && x.SaleState != "Returned" && x.DepartmentId == depId);
                    break;
                ///////////////////   last month report ////////////////////////
                case 3:
                    sale = db.Sale.Where(x => x.SaleDate.Value.Month == (DateTime.Now.Month - 1) && x.SaleDate.Value.Year == DateTime.Now.Year && x.SaleState != "Returned" && x.DepartmentId == depId);

                    break;
                ///////////////////  Current Year's report  ////////////
                case 4:
                    var currentYear = DateTime.Now.Year;

                    sale = db.Sale.Where(x => x.SaleDate.Value.Year == currentYear && x.SaleState != "Returned" && x.DepartmentId == depId);
                    break;
                //////////////////   Last year's report ///////////////
                case 5:
                    sale = db.Sale.Where(x => x.SaleDate.Value.Year == (DateTime.Now.Year - 1) && x.SaleState != "Returned" && x.DepartmentId == depId);
                    break;
            }




            return Json(sale);
        }

        [Authorize(Roles = "Admin Department")]
        public IActionResult ManualReprot(AllViewModel model, string BillNo)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var sale = db.Sale.Where(x => x.SaleId > 0 && x.SaleState != "Returned" && x.DepartmentId == depId);
            if (BillNo == "" || BillNo == null)
            {
                sale = db.Sale.Where(x => x.SaleDate >= model.manulReport.FromDate.Date && x.SaleDate <=
                                     model.manulReport.ToDate.Date && x.SaleState != "Returned" && x.DepartmentId == depId);
            }
            else
            {
                sale = db.Sale.Where(x => x.BillNo == BillNo && x.SaleState != "Returned" && x.DepartmentId == depId);
            }

            return Json(sale);

        }
    }
}
