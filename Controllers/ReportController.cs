using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralSalesDb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Admin Department,Finance Department")]
    public class ReportController : Controller
    {

        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> userManager;

        int depId = 0;

        public ReportController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }


        ///// New Report Code ///////
        public IActionResult Index()
        {
            return View();
        }

        ///// Date Wise Report ///////
        public JsonResult FetchGeneralReportData(DateTime fromDate, DateTime toDate)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {


                var stockPurchase = (from p in db.Purchase
                                     where p.DepartmentId == depId && p.Status != "Returned" && p.PurchaseDate >= fromDate && p.PurchaseDate <= toDate
                                     select new
                                     {
                                         price =( p.Quantity + (p.Quantity2 / p.InUnit)) * (p.Price +p.Expense),
                                         currency = p.CurrencyId,
                                     }).ToList();


                var sale = (from p in db.Sale
                            where p.DepartmentId == depId && p.SaleState != "Returned" && p.SaleDate >= fromDate && p.SaleDate <= toDate
                            select new
                            {
                                price = (p.Quantity * p.SalePrice) - p.Discount,
                                benifit = ((p.Quantity * p.SalePrice) - p.Discount) - (p.Quantity * p.PurchasePrice),
                                currency = p.CurrencyId,
                            }).ToList();

                var expense = (from p in db.Expence
                               where p.DepartmentId == depId  && p.ExpenceDate >= fromDate && p.ExpenceDate <= toDate
                               select new
                               {
                                   price = p.ExpenceAmount,
                                   currency = p.CurrencyId,
                               }).ToList();


                var data = new
                {
                    stockPurchase,
                    sale,
                    expense,
                };

                return Json(data);
            }
            else
            {


                var stockPurchase = (from p in db.Purchase
                                     where p.DepartmentId == depId && p.Status != "Returned" && p.PurchaseDate.Value.Year == DateTime.Now.Year && p.PurchaseDate.Value.Month == DateTime.Now.Month
                                     select new
                                     {
                                         price = (p.Quantity + (p.Quantity2 / p.InUnit)) * (p.Price + p.Expense),
                                         currency = p.CurrencyId,
                                     }).ToList();

                var sale = (from p in db.Sale
                            where p.DepartmentId == depId && p.SaleState != "Returned" && p.SaleDate.Value.Year == DateTime.Now.Year && p.SaleDate.Value.Month == DateTime.Now.Month
                            select new
                            {
                                price = (p.Quantity * p.SalePrice) - p.Discount,
                                benifit = ((p.Quantity * p.SalePrice) - p.Discount) - (p.Quantity * p.PurchasePrice),
                                currency = p.CurrencyId,
                            }).ToList();


                var expense = (from p in db.Expence
                               where p.DepartmentId == depId  && p.ExpenceDate.Value.Year == DateTime.Now.Year && p.ExpenceDate.Value.Month == DateTime.Now.Month
                               select new
                               {
                                   price = p.ExpenceAmount,
                                   currency = p.CurrencyId,
                               }).ToList();


                var data = new
                {
                    stockPurchase,
                    sale,
                    expense,
                };

                return Json(data);
            }
        }

        ///// Purchase Report ///////
        public IActionResult PurchaseReportDetails()
        {
            return View();
        }

        ///// Sale Report ///////
        public IActionResult SaleReportDetails()
        {
            return View();
        }

        ///// All Deals Info ///////
        public async Task<JsonResult> FetchDealsInfo()
        {

            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var AllBanks = await db.Bank.Where(x => x.Status != "Locked" && x.DepartmentId == depId).ToListAsync();

            var afBank = 0.0;
            var pkBank = 0.0;
            var usBank = 0.0;

            foreach (var item in AllBanks)
            {
                afBank += await db.BankDeal.Where(x => x.BankId == item.BankId && x.CurrencyId == 1).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefaultAsync();
                pkBank += await db.BankDeal.Where(x => x.BankId == item.BankId && x.CurrencyId == 2).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefaultAsync();
                usBank += await db.BankDeal.Where(x => x.BankId == item.BankId && x.CurrencyId == 3).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefaultAsync();
            }

            var AllCus = await db.Customer.Where(x => x.Status != "Locked" && x.DepartmentId == depId).ToListAsync();

            var afCus = 0.0;
            var pkCus = 0.0;
            var usCus = 0.0;

            foreach (var item in AllCus)
            {
                afCus += await db.CustomerDeal.Where(x => x.CustomerId == item.CustomerId && x.CurrencyId == 1).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
                pkCus += await db.CustomerDeal.Where(x => x.CustomerId == item.CustomerId && x.CurrencyId == 2).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
                usCus += await db.CustomerDeal.Where(x => x.CustomerId == item.CustomerId && x.CurrencyId == 3).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
            }

            var AllDealer = await db.Dealer.Where(x => x.Status != "Locked" && x.DepartmentId == depId).ToListAsync();

            var afDeal = 0.0;
            var pkDeal = 0.0;
            var usDeal = 0.0;

            foreach (var item in AllDealer)
            {
                afDeal += await db.DealerDeal.Where(x => x.DealerId == item.DealerId && x.CurrencyId == 1).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
                pkDeal += await db.DealerDeal.Where(x => x.DealerId == item.DealerId && x.CurrencyId == 2).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
                usDeal += await db.DealerDeal.Where(x => x.DealerId == item.DealerId && x.CurrencyId == 3).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefaultAsync();
            }



            var sale = (from p in db.Sale
                        where p.DepartmentId == depId && p.SaleState != "Returned" 
                        select new
                        {
                            benifit = ((p.Quantity * p.SalePrice) - p.Discount) - (p.Quantity * p.PurchasePrice),
                            currency = p.CurrencyId,
                        }).ToList();

            var afCr = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 1).Sum(x => x.Credit);
            var afDb = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 1).Sum(x => x.Debit);

            var pkCr = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 2).Sum(x => x.Credit);
            var pkDb = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 2).Sum(x => x.Debit);

            var usCr = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 3).Sum(x => x.Credit);
            var usDb = db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 3).Sum(x => x.Debit);

            var afCash = afCr - afDb;
            var pkCash = pkCr - pkDb;
            var usCash = usCr - usDb;

            var data = new
            {
                afBank,
                pkBank,
                usBank,

                afCus,
                pkCus,
                usCus,

                afDeal,
                pkDeal,
                usDeal,

                afCash,
                pkCash,
                usCash,

                sale
            };

            return Json(data);

        }

        ///// All Stocks Info ///////
        public async Task<JsonResult> FetchStocksInfo()
        {

            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var afStock = 0.0;
            var pkStock = 0.0;
            var usStock = 0.0;

            var AllStock = await db.StockItems.Where(x => x.Quantity > 0 && x.DepartmentId == depId).ToListAsync();


            foreach (var item in AllStock)
            {
                if (item.CurrencyId == 1)
                {
                    afStock += (item.Quantity/item.InUnit) * item.Price;
                }
                else if (item.CurrencyId == 2)
                {
                    pkStock += (item.Quantity / item.InUnit) * item.Price;
                }
                else if (item.CurrencyId == 3)
                {
                    usStock += (item.Quantity / item.InUnit) * item.Price;
                }
            }

            var data = new
            {
                
                afStock,
                pkStock,
                usStock,
            };

            return Json(data);

        }

        ///// All Stocks Info ///////
        public JsonResult FetchAssetsInfo()
        {

            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var afInvCr = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 1 && x.Type == "وسایل").Sum(x => x.Credit);
            var afInvDb = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 1 && x.Type == "وسایل").Sum(x => x.Debit);

            var pkInvCr = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 2 && x.Type == "وسایل").Sum(x => x.Credit);
            var pkInvDb = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 2 && x.Type == "وسایل").Sum(x => x.Debit);

            var usInvCr = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 3 && x.Type == "وسایل").Sum(x => x.Credit);
            var usInvDb = db.InvestMoney.Where(x => x.DepartmentId == depId && x.CurrencyId == 3 && x.Type == "وسایل").Sum(x => x.Debit);

            var afLast = db.Assets.Where(x => x.Dates == DateTime.Now.Year - 1 && x.DepartmentId == depId).Select(x => x.AfAsset).FirstOrDefault();
            var pkLast = db.Assets.Where(x => x.Dates == DateTime.Now.Year - 1 && x.DepartmentId == depId).Select(x => x.PkAsset).FirstOrDefault();
            var usLast = db.Assets.Where(x => x.Dates == DateTime.Now.Year - 1 && x.DepartmentId == depId).Select(x => x.UsAsset).FirstOrDefault();

            var afInv = afInvCr - afInvDb;
            var pkInv = pkInvCr - pkInvDb;
            var usInv = usInvCr - usInvDb;

            var data = new
            {
                afInv,
                pkInv,
                usInv,

                afLast,
                pkLast,
                usLast,
            };

            return Json(data);

        }


        /// todays report view ///
        public IActionResult Daily()
        {
            return View();
        }


        ///// Date Wise daily  Report ///////
        public async Task<JsonResult> FetchDailyReportData(DateTime fromDate, DateTime toDate)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {


                var purchase = await db.DealerDeal.Where(x=> x.Date >= fromDate && x.Date <= toDate).ToListAsync();

                var sale = await db.CustomerDeal.Where(x => x.Date >= fromDate && x.Date <= toDate).ToListAsync();

                var bank = await db.BankDeal.Where(x => x.Date >= fromDate && x.Date <= toDate).ToListAsync();


                var data = new
                {
                    purchase,
                    sale,
                    bank
                };

                return Json(data);
            }
            else
            {
                var purchase = await db.DealerDeal.Where(x =>  x.Date == DateTime.Now.Date).ToListAsync();

                var sale = await db.CustomerDeal.Where(x =>  x.Date == DateTime.Now.Date).ToListAsync();

                var bank = await db.BankDeal.Where(x =>  x.Date == DateTime.Now.Date).ToListAsync();


                var data = new
                {
                    purchase,
                    sale,
                    bank
                };

                return Json(data);
            }
        }

        /// item report view ///
        public IActionResult Item()
        {
            var items = (from d in db.Item
                         select new SelectListItem()
                         {
                             Text = d.Name,
                             Value = d.ItemId.ToString()
                         }).ToList();
            ViewBag.items = items;

            return View();
        }

        ///// Date Wise daily  Report ///////
        public async Task<JsonResult> FetchItemInfo(DateTime fromDate, DateTime toDate,int itemId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {


                var purchase =await (from p in db.Purchase
                                     where p.DepartmentId == depId && p.Status != "Returned" && p.PurchaseDate >= fromDate && p.PurchaseDate <= toDate && p.ItemId==itemId
                                     select new
                                     {
                                         price = (p.Quantity + (p.Quantity2 / p.InUnit)) * (p.Price + p.Expense),
                                         qty = p.Quantity,
                                         qty2 = p.Quantity2,
                                         currencyId = p.CurrencyId,
                                     }).ToListAsync();

                var sale = await (from p in db.Sale
                            where p.DepartmentId == depId && p.SaleState != "Returned" && p.SaleDate >= fromDate && p.SaleDate <= toDate && p.ItemId==itemId
                            select new
                            {
                                price = (p.Quantity * p.SalePrice) - p.Discount,
                                qty = p.Quantity,
                                type= p.SaleType,
                                benifit = ((p.Quantity * p.SalePrice) - p.Discount) - (p.Quantity * p.PurchasePrice),
                                currencyId = p.CurrencyId,
                            }).ToListAsync();

                var stock = await (from p in db.StockItems
                            where p.DepartmentId == depId  && p.ItemId == itemId && p.Quantity > 0
                             select new
                            {
                                 price = p.Quantity * (p.Price / p.InUnit),
                                 qty = p.Quantity,
                                 inunit = p.InUnit,
                                 currencyId = p.CurrencyId,
                            }).ToListAsync();


                var data = new
                {
                    purchase,
                    sale,
                    stock
                };

                return Json(data);
            }
            else
            {
                var purchase = await (from p in db.Purchase
                                     where p.DepartmentId == depId && p.Status != "Returned" && p.ItemId==itemId
                                     select new
                                     {
                                         price = (p.Quantity + (p.Quantity2 / p.InUnit)) * (p.Price + p.Expense),
                                         qty = p.Quantity,
                                         qty2 = p.Quantity2,
                                         currencyId = p.CurrencyId,
                                     }).ToListAsync();

                var sale = await (from p in db.Sale
                            where p.DepartmentId == depId && p.SaleState != "Returned" && p.ItemId == itemId
                            select new
                            {
                                price = (p.Quantity * p.SalePrice) - p.Discount,
                                qty = p.Quantity,
                                type = p.SaleType,
                                benifit = ((p.Quantity * p.SalePrice) - p.Discount) - (p.Quantity * p.PurchasePrice),
                                currencyId = p.CurrencyId,
                            }).ToListAsync();


                var stock = await (from p in db.StockItems
                             where p.DepartmentId == depId  && p.ItemId == itemId && p.Quantity>0
                             select new
                             {
                                 price = p.Quantity * (p.Price / p.InUnit),
                                 qty =p.Quantity,
                                 inunit = p.InUnit,
                                 currencyId = p.CurrencyId,
                             }).ToListAsync();


                var data = new
                {
                    purchase,
                    sale,
                    stock
                };

                return Json(data);
            }
        }

        public IActionResult Expense()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ManualReprot(ManulReprotViewModel model, int currencyId)
        {
            if (ModelState.IsValid)
            {
                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                var category = db.ExpenceType.Where(cat => cat.DepartmentId == depId).ToList();

                Dictionary<string, double> dictMonthlySum = new Dictionary<string, double>();

                foreach (var item in category)
                {
                    var amount = db.Expence.Where
                   (cat => cat.ExpenceDate >= model.FromDate.Date && cat.ExpenceDate <= model.ToDate.Date && cat.CurrencyId == currencyId  && cat.ExpenceTypeId == item.ExpenceTypeId && cat.DepartmentId == depId)
                   .Select(cat => cat.ExpenceAmount)
                   .Sum();

                    dictMonthlySum.Add(item.ExpenceType1, amount);
                }

                var salaries = db.SalaryPayment.Where
                    (cat => cat.PaidDate >= model.FromDate.Date && cat.CurrencyId == currencyId && cat.PaidDate <= model.ToDate.Date && cat.DepartmentId == depId)
                    .Select(cat => cat.PaidAmount).Sum();

                dictMonthlySum.Add("معاشات", salaries);


                return new JsonResult(dictMonthlySum);


            }
            return View("Error");
        }

        public JsonResult GetCurrentMonthExpense(int currencyId)
        {

            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            Dictionary<string, double> dictMonthlySum = new Dictionary<string, double>();

            var category = db.ExpenceType.Where(cat => cat.DepartmentId == depId).ToList();

            foreach (var item in category)
            {
                var amount = db.Expence.Where(cat => cat.ExpenceDate.Value.Year == DateTime.Now.Date.Year && cat.ExpenceDate.Value.Month == DateTime.Now.Date.Month  && cat.CurrencyId == currencyId && cat.ExpenceTypeId == item.ExpenceTypeId && cat.DepartmentId == depId)
               .Select(cat => cat.ExpenceAmount)
               .Sum();

                dictMonthlySum.Add(item.ExpenceType1, amount);
            }

            var salaries = db.SalaryPayment.Where(cat => cat.PaidDate.Value.Year == DateTime.Now.Date.Year && cat.PaidDate.Value.Month == DateTime.Now.Date.Month  && cat.CurrencyId == currencyId && cat.DepartmentId == depId).Select(cat => cat.PaidAmount).Sum();

            dictMonthlySum.Add("معاشات", salaries);

            return new JsonResult(dictMonthlySum);
        }

        public IActionResult Salaries()
        {
            return View();
        }

        public JsonResult LoadCurrentMonthsSalaries()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = (from s in db.SalaryPayment
                       join e in db.Employee on s.EmployeeId equals e.EmployeeId
                       where s.PaidDate.Value.Year == DateTime.Now.Year && s.PaidDate.Value.Month == DateTime.Now.Month && s.DepartmentId == depId
                       select new
                       {
                           name = e.Name,
                           phone = e.Phone,
                           email = e.Email,
                           paidAmount = s.PaidAmount,
                           paidDate = Convert.ToDateTime(s.PaidDate).Date.ToShortDateString(),
                           paidBy = db.Employee.Where(d => d.Email == s.PaidBy).Select(r => r.Name).FirstOrDefault(),
                       }).ToList();
            return new JsonResult(rec);
        }

        public IActionResult ManualSalaryReprot(ManulReprotViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = userManager.GetUserAsync(User).Result.DepartmentId;
                var monthStart = model.FromDate.Date;
                var monthEnd = model.ToDate.Date;

                var rec = (from s in db.SalaryPayment
                           join e in db.Employee on s.EmployeeId equals e.EmployeeId
                           where s.PaidDate >= monthStart && s.PaidDate <= monthEnd && s.DepartmentId == depId
                           select new
                           {
                               name = e.Name,
                               phone = e.Phone,
                               email = e.Email,
                               paidAmount = s.PaidAmount,
                               paidDate = Convert.ToDateTime(s.PaidDate).Date.ToShortDateString(),
                               paidBy = db.Employee.Where(d => d.Email == s.PaidBy).Select(r => r.Name).FirstOrDefault(),
                           }).ToList();
                return new JsonResult(rec);
            }
            return View("Error");
        }

    }
}