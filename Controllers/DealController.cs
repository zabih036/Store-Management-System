using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Finance Department,Admin Department,Super Admin")]
    public class DealController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment hostingEnvironment;

        int depId = 0;
        public DealController(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealers = (from d in db.Dealer
                           where d.DepartmentId == depId && d.Status == "Unlocked"
                           select new SelectListItem()
                           {
                               Text = d.Name + " / " + d.Fname,
                               Value = d.DealerId.ToString()
                           }).ToList();
            ViewBag.dealers = dealers;

            var customers = (from d in db.Customer
                             where d.DepartmentId == depId && d.Status == "Unlocked"
                             select new SelectListItem()
                             {
                                 Text = d.Name + " / " + d.Fname,
                                 Value = d.CustomerId.ToString()
                             }).ToList();
            ViewBag.customers = customers;


            var expenseType = (from d in db.ExpenceType
                               where d.DepartmentId == depId
                               select new SelectListItem()
                               {
                                   Text = d.ExpenceType1,
                                   Value = d.ExpenceTypeId.ToString()
                               }).ToList();
            ViewBag.expenseType = expenseType;

            var bank = (from d in db.Bank
                        where d.DepartmentId == depId
                        select new SelectListItem()
                        {
                            Text = d.Name,
                            Value = d.BankId.ToString()
                        }).ToList();
            ViewBag.bank = bank;

            return View();
        }

        public async Task<JsonResult> FetchSerialNumber()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            long serialNumber = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).Select(x => x.CashSerial).FirstOrDefault();

            if (serialNumber == 0)
            {
                SerialTable serial = new SerialTable
                {
                    SaleSerial = 1,
                    DepartmentId = depId,
                };

                db.SerialTable.Add(serial);
                await db.SaveChangesAsync();

                return Json(new
                {
                    serialNumber = 1,
                });
            }

            return Json(new
            {
                serialNumber = serialNumber,
            });
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRecord(long dealId, string record,int dealerId,int currencyId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            if (record == "1")
            {
                var deals = await db.DealerDeal.Where(x => x.DealId >= dealId && x.DealerId==dealerId && x.CurrencyId == currencyId).ToListAsync();

                var deal = await db.DealerDeal.Where(x => x.DealId == dealId && x.DealerId == dealerId && x.CurrencyId == currencyId).FirstOrDefaultAsync();

                var debit = deal.Debit;
                var credit = deal.Credit;

                foreach (var item in deals)
                {
                    item.Balance += debit;
                    item.Balance -= credit;

                    db.DealerDeal.Update(item);
                }

                if (deals.Count == 1)
                {
                    var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                    serial.CashSerial -= 1;

                    db.SerialTable.Update(serial);
                }

                var cash = await db.CashInHand.Where(x => x.DealId == dealId && x.CurrencyId == currencyId).FirstOrDefaultAsync();

                if (cash != null)
                {
                    db.CashInHand.Remove(cash);
                }

                db.DealerDeal.Remove(deal);

                await db.SaveChangesAsync();

                return Json("کهاتدار ریکارډ حذف شو");
            }
            else
            {
                var deals = await db.CustomerDeal.Where(x => x.DealId >= dealId && x.CustomerId == dealerId && x.CurrencyId == currencyId).ToListAsync();

                var deal = await db.CustomerDeal.Where(x => x.DealId == dealId && x.CustomerId == dealerId && x.CurrencyId == currencyId).FirstOrDefaultAsync();

                var debit = deal.Debit;
                var credit = deal.Credit;

                foreach (var item in deals)
                {
                    item.Balance -= debit;
                    item.Balance += credit;

                    db.CustomerDeal.Update(item);
                }

                if (deals.Count == 1)
                {
                    var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                    serial.CashSerial -= 1;

                    db.SerialTable.Update(serial);
                }


                var cash = await db.CashInHand.Where(x => x.CusDealId == dealId  && x.CurrencyId == currencyId).FirstOrDefaultAsync();

                if (cash != null)
                {
                    db.CashInHand.Remove(cash);
                }

                db.CustomerDeal.Remove(deal);


                await db.SaveChangesAsync();

                return Json("مشترې ریکارډ حذف شو");
            }
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddDealInfo(DealViewModel model, [FromForm] IFormFile image)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var email = userManager.GetUserAsync(User).Result.Email;

                    depId = userManager.GetUserAsync(User).Result.DepartmentId;


                    double cr = 0.0;
                    double cr2 = 0.0;
                    double deb = 0.0;
                    double deb2 = 0.0;

                    var balance = 0.0;

                    var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId==1).FirstOrDefault();

                    var sno = serial.CashSerial;

                    serial.CashSerial += 1;

                    db.SerialTable.Update(serial);

                    if (model.CustomerId != 0)
                    {

                        if (model.Type == "Debit")
                        {
                            deb = model.Amount;
                            deb2 = model.Amount2;

                            balance = model.Amount;
                        }
                        else
                        {
                            cr = model.Amount;
                            cr2 = model.Amount2;

                            balance = -model.Amount;
                        }

                        var dealing = await db.CustomerDeal.Where(x =>
                        x.CustomerId == model.CustomerId
                        && x.CurrencyId == model.CurrencyId
                        && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();



                        if (dealing != null)
                        {
                            balance += dealing.Balance;
                        }

                        CustomerDeal deal = new CustomerDeal
                        {
                            BillNo = model.BillNo,
                            Debit = deb,
                            Credit = cr,
                            Balance = balance,
                            CustomerId = model.CustomerId,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.CurrencyId,
                            Employee = email,
                            Type = model.Type,
                            Detail = model.Detail,
                            SerialNumber = sno,
                            DepartmentId = depId,

                        };

                        await db.CustomerDeal.AddAsync(deal);
                        await db.SaveChangesAsync();

                        CashInHand cash = new CashInHand
                        {
                            Debit = deb2,
                            Credit = cr2,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.Currency2Id,
                            CusDealId = deal.DealId,
                            Description = model.Detail,
                            DepartmentId = depId
                        };

                        await db.CashInHand.AddAsync(cash);

                        await db.SaveChangesAsync();

                       return Json("کهاتې معلومات ذخیره شول");
                    }
                    else
                    {

                        if (model.Type == "Debit")
                        {
                            deb = model.Amount;
                            deb2 = model.Amount2;

                            balance = -model.Amount;
                        }
                        else
                        {
                            cr = model.Amount;
                            cr2 = model.Amount2;

                            balance = model.Amount;
                        }

                        var dealing = await db.DealerDeal.Where(x =>
                         x.DealerId == model.DealerId
                         && x.CurrencyId == model.CurrencyId
                         && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();


                        if (dealing != null)
                        {
                            balance += dealing.Balance;
                        }

                        DealerDeal deal = new DealerDeal
                        {
                            BillNo = model.BillNo,
                            Debit = deb,
                            Credit = cr,
                            Balance = balance,
                            DealerId = model.DealerId,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.CurrencyId,
                            Employee = email,
                            Type = model.Type,
                            Detail = model.Detail,
                            SerialNumber = sno,
                            DepartmentId = depId,

                        };

                        await db.DealerDeal.AddAsync(deal);

                        await db.SaveChangesAsync();

                        CashInHand cash = new CashInHand
                        {
                            Debit = deb2,
                            Credit = cr2,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.Currency2Id,
                            DealId = deal.DealId,
                            Description = model.Detail,
                            DepartmentId = depId
                        };

                        await db.CashInHand.AddAsync(cash);

                        await db.SaveChangesAsync();

                        return Json("کهاتې معلومات ذخیره شول");
                    }
                }
                catch (Exception )
                {

                   return Json(" ریکارډ ذخیره نه شو مهربانی له مخې بیا کوښښ وکړئ ");
                }

            }

            return Json(" ریکارډ ذخیره نه شو مهربانی له مخې بیا کوښښ وکړئ ");

        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> SaveExpense(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = userManager.GetUserAsync(User).Result.Email;

                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                Expence c = new Expence()
                {
                    ExpenceAmount = model.Expense,
                    CurrencyId = model.ECurrencyId,
                    ExpenceTypeId = model.ExpenseTypeId,
                    ExpenceDate = model.ExpenseDate,
                    Detail = model.Detail,
                    DepartmentId = depId,
                    Employee = email
                };

                await db.Expence.AddAsync(c);
                await db.SaveChangesAsync();

                CashInHand ch = new CashInHand()
                {
                    Debit = model.Expense,
                    Credit = 0,
                    Date = model.ExpenseDate,
                    CurrencyId = model.ECurrencyId,
                    Description = "expense",
                    ExpenseId = c.ExpenceId,
                    DepartmentId = depId
                };
                await db.CashInHand.AddAsync(ch);

                await db.SaveChangesAsync();
                return Json(" مصرف ریکارډ ذخیره شو.");
            }

            return View("Error");
        }

        public JsonResult LoadExpense()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = (from i in db.Expence
                       where i.DepartmentId == depId
                       select new
                       {
                           expenceId = i.ExpenceId,
                           expenceAmount = i.ExpenceAmount,
                           currency = i.Currency.Currency1,
                           expenceType = i.ExpenceType.ExpenceType1,
                           expenceDateShow = Convert.ToDateTime(i.ExpenceDate).ToShortDateString(),
                           detail = i.Detail ?? "",
                           expenceTypeId = i.ExpenceTypeId
                       }).ToList().OrderByDescending(d => d.expenceId);
            return Json(rec);
        }

        /// dealer /// 
        public IActionResult Dealers()
        {
            return View();
        }

        public async Task<JsonResult> FetchDealers()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealers = await (from d in db.Dealer
                                 where d.DepartmentId == depId && d.Status == "Unlocked"
                                 select new
                                 {
                                     dealerId = d.DealerId,
                                     name = d.Name,
                                     fname = d.Fname,
                                     address = d.Address,
                                     phone = d.Phone,
                                 }).OrderByDescending(x => x.dealerId).ToListAsync();
            return Json(dealers);
        }

        public async Task<JsonResult> DealersDealInfo()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var DealInfo = await (from d in db.DealerDeal
                                  join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                                  join de in db.Dealer on d.DealerId equals de.DealerId
                                  where de.Status == "Unlocked" && d.DepartmentId == depId
                                  select new
                                  {
                                      dealerid = d.DealerId,
                                      billNo = d.BillNo,
                                      dealid = d.DealId,
                                      name = de.Name,
                                      fname = de.Fname,
                                      phone = de.Phone,
                                      image = d.Image,
                                      debit = d.Debit,
                                      credit = d.Credit,
                                      balance = d.Balance,
                                      currencyid = d.CurrencyId,
                                      currency = cu.Currency1,
                                      employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                      date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                      details = d.Detail ?? "",
                                      serial = d.SerialNumber,
                                      inWord = d.InWord,
                                      emPhone = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Phone).FirstOrDefault(),
                                  }).OrderByDescending(r => r.dealid).ToListAsync();

            return Json(DealInfo);
        }

        public JsonResult FetchDealerDealInfo(int dealerId, int currencyId)
        {
            var DealInfo = (from d in db.DealerDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            join de in db.Dealer on d.DealerId equals de.DealerId
                            where d.DealerId == dealerId && d.CurrencyId == currencyId && de.Status == "Unlocked"
                            select new
                            {
                                dealerId = d.DealerId,
                                billNo = d.BillNo,
                                dealid = d.DealId,
                                name = de.Name,
                                fname = de.Fname,
                                phone = de.Phone,
                                image = d.Image,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).Take(50);

            return Json(DealInfo);
        }

        public JsonResult FetchDealerDealInfoByDate(int dealerId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var DealInfo = (from d in db.DealerDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            join de in db.Dealer on d.DealerId equals de.DealerId
                            where d.DealerId == dealerId && d.CurrencyId == currencyId && d.Date >= fromDate && d.Date <= toDate
                            select new
                            {
                                dealerid = d.DealerId,
                                billNo = d.BillNo,
                                dealid = d.DealId,
                                name = de.Name,
                                fname = de.Fname,
                                phone = de.Phone,
                                image = d.Image,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).ToList();

            return Json(DealInfo);
        }

        public IActionResult DealersRemainList()
        {
            return View();
        }

        public JsonResult TotalRemainFromDealer()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var af = (from d in db.Dealer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.DealerDeal.Where(x => x.CurrencyId == 1 && x.DealerId == d.DealerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()
                      }).ToList();
            var pk = (from d in db.Dealer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.DealerDeal.Where(x => x.CurrencyId == 2 && x.DealerId == d.DealerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var us = (from d in db.Dealer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.DealerDeal.Where(x => x.CurrencyId == 3 && x.DealerId == d.DealerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var rec = new
            {
                af,
                pk,
                us,
            };
            return Json(rec);
        }

        /// customers ///
        public IActionResult Customers()
        {
            return View();
        }

        public async Task<JsonResult> FetchCustomers()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var customers = await (from cus in db.Customer
                                   where cus.DepartmentId == depId && cus.Status == "Unlocked"
                                   select new
                                   {
                                       customerid = cus.CustomerId,
                                       name = cus.Name,
                                       fname = cus.Fname,
                                       address = cus.Address,
                                       phone = cus.Phone,
                                   }).OrderByDescending(x => x.customerid).ToListAsync();
            return Json(customers);
        }

        public JsonResult CustomersDealInfo()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var DealInfo = (from d in db.CustomerDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            where d.DepartmentId == depId && d.Customer.Status == "Unlocked"
                            select new
                            {
                                customerid = d.CustomerId,
                                billNo = d.BillNo,
                                dealid = d.DealId,
                                name = d.Customer.Name,
                                fname = d.Customer.Fname,
                                phone = d.Customer.Phone,
                                image = d.Image,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                dealerid = d.CustomerId,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                                serial = d.SerialNumber,
                                inWord = d.InWord,
                                emPhone = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Phone).FirstOrDefault(),
                            }).ToList().OrderByDescending(r => r.dealid);

            return Json(DealInfo);
        }

        public JsonResult FetchCustomerDealInfo(int customerId, int currencyId)
        {

            var DealInfo = (from d in db.CustomerDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            where d.CustomerId == customerId && d.CurrencyId == currencyId
                            select new
                            {
                                customerid = d.CustomerId,
                                billNo = d.BillNo,
                                dealid = d.DealId,
                                name = d.Customer.Name,
                                fname = d.Customer.Fname,
                                phone = d.Customer.Phone,
                                image = d.Image,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                dealerid = d.CustomerId,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).Take(50);

            return Json(DealInfo);
        }

        public JsonResult FetchDealInfoByDate(int customerId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var DealInfo = (from d in db.CustomerDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            where d.CustomerId == customerId && d.CurrencyId == currencyId && d.Date >= fromDate && d.Date <= toDate
                            select new
                            {
                                customerid = d.CustomerId,
                                billNo = d.BillNo,
                                dealid = d.DealId,
                                name = d.Customer.Name,
                                fname = d.Customer.Fname,
                                phone = d.Customer.Phone,
                                image = d.Image,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                dealerid = d.CustomerId,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).ToList();

            return Json(DealInfo);
        }

        public IActionResult CustomerRemainList()
        {
            return View();
        }

        public JsonResult TotalRemainOnCustomer()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var af = (from d in db.Customer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.CustomerDeal.Where(x => x.CurrencyId == 1 && x.CustomerId == d.CustomerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()
                      }).ToList();
            var pk = (from d in db.Customer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.CustomerDeal.Where(x => x.CurrencyId == 2 && x.CustomerId == d.CustomerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var us = (from d in db.Customer
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          fname = d.Fname,
                          province = d.Address,
                          phone = d.Phone,
                          balance = db.CustomerDeal.Where(x => x.CurrencyId == 3 && x.CustomerId == d.CustomerId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var rec = new
            {
                af,
                pk,
                us,
            };
            return Json(rec);
        }

        /// Banks /// 
        public IActionResult Banks()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealers = (from d in db.Dealer
                           where d.DepartmentId == depId && d.Status == "Unlocked"
                           select new SelectListItem()
                           {
                               Text = d.Name + " / " + d.Fname,
                               Value = d.DealerId.ToString()
                           }).ToList();
            ViewBag.dealers = dealers;

            var customers = (from d in db.Customer
                             where d.DepartmentId == depId && d.Status == "Unlocked"
                             select new SelectListItem()
                             {
                                 Text = d.Name + " / " + d.Fname,
                                 Value = d.CustomerId.ToString()
                             }).ToList();
            ViewBag.customers = customers;

            return View();
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddBankDeal(BankDealViewModel model, string cash)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var email = userManager.GetUserAsync(User).Result.Email;

                    depId = userManager.GetUserAsync(User).Result.DepartmentId;

                    double cr = 0.0;
                    double deb = 0.0;

                    var balance = 0.0;

                    if (model.Type == "Debit")
                    {
                        deb = model.Amount;

                        balance = -model.Amount;
                    }
                    else
                    {
                        cr = model.Amount;

                        balance = model.Amount;
                    }

                    var dealing = await db.BankDeal.Where(x =>
                    x.BankId == model.BankId
                    && x.CurrencyId == model.CurrencyId
                    && x.DepartmentId == depId).OrderByDescending(x => x.BankDealId).FirstOrDefaultAsync();

                    if (dealing != null)
                    {
                        balance += dealing.Balance;
                    }

                    BankDeal deal = new BankDeal
                    {
                        Debit = deb,
                        Credit = cr,
                        Balance = balance,
                        BankId = model.BankId,
                        Date = DateTime.Now.Date,
                        CurrencyId = model.CurrencyId,
                        Employee = email,
                        Detail = model.Detail,
                        DepartmentId = depId,
                    };

                    await db.BankDeal.AddAsync(deal);
                    await db.SaveChangesAsync();

                    if (cash == "on")
                    {
                        CashInHand cashIn = new CashInHand
                        {
                            Debit = deb,
                            Credit = cr,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.CurrencyId,
                            BankDealId = deal.BankDealId,
                            Description = model.Detail,
                            DepartmentId = depId
                        };

                        await db.CashInHand.AddAsync(cashIn);

                        await db.SaveChangesAsync();
                    }

                    return Json(" معلومات ذخیره شول");

                }
                catch (Exception e)
                {

                    return Json("اشتباه " + e.Message);
                }
            }
            return Json("اشتباه ");
        }

        public async Task<JsonResult> FetchBanks()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealers = await (from d in db.Bank
                                 where d.DepartmentId == depId && d.Status == "Unlocked"
                                 select new
                                 {
                                     bankId = d.BankId,
                                     name = d.Name,
                                     address = d.Address,
                                     phone = d.Phone,

                                 }).OrderByDescending(x => x.bankId).ToListAsync();
            return Json(dealers);
        }

        public async Task<JsonResult> BanksDealInfo()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var DealInfo = await (from d in db.BankDeal
                                  join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                                  join de in db.Bank on d.BankId equals de.BankId
                                  where de.Status == "Unlocked" && d.DepartmentId == depId
                                  select new
                                  {
                                      bankId = d.BankId,
                                      dealid = d.BankDealId,
                                      name = de.Name,
                                      phone = de.Phone,
                                      address = de.Address,
                                      debit = d.Debit,
                                      credit = d.Credit,
                                      balance = d.Balance,
                                      currency = cu.Currency1,
                                      date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                      details = d.Detail ?? "",
                                  }).OrderByDescending(r => r.dealid).ToListAsync();

            return Json(DealInfo);
        }

        public JsonResult FetchBankDealInfo(int bankId, int currencyId)
        {
            var DealInfo = (from d in db.BankDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            join de in db.Bank on d.BankId equals de.BankId
                            where d.BankId == bankId && d.CurrencyId == currencyId && de.Status == "Unlocked"
                            select new
                            {
                                bankId = d.BankId,
                                dealid = d.BankDealId,
                                name = de.Name,
                                phone = de.Phone,
                                address = de.Address,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                serial = d.SerialNo,
                                currency2 = d.Currency2,
                                dealer = d.DealerName,
                                amount2 = d.Amount,
                                type = d.Type,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).Take(50);

            return Json(DealInfo);
        }

        public JsonResult FetchBankDealInfoByDate(int bankId, int currencyId, DateTime fromDate, DateTime toDate)
        {
            var DealInfo = (from d in db.BankDeal
                            join cu in db.Currency on d.CurrencyId equals cu.CurrencyId
                            join de in db.Bank on d.BankId equals de.BankId
                            where d.BankId == bankId && d.CurrencyId == currencyId && d.Date >= fromDate && d.Date <= toDate && de.Status == "Unlocked"
                            select new
                            {
                                bankId = d.BankId,
                                dealid = d.BankDealId,
                                name = de.Name,
                                phone = de.Phone,
                                address = de.Address,
                                debit = d.Debit,
                                credit = d.Credit,
                                balance = d.Balance,
                                currencyid = d.CurrencyId,
                                currency = cu.Currency1,
                                serial = d.SerialNo,
                                currency2 = d.Currency2,
                                dealer = d.DealerName,
                                amount2 = d.Amount,
                                type = d.Type,
                                employee = db.Employee.Where(x => x.Email == d.Employee).Select(x => x.Name).FirstOrDefault(),
                                date = Convert.ToDateTime(d.Date).ToShortDateString(),
                                details = d.Detail ?? "",
                            }).ToList();

            return Json(DealInfo);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeletedBankDeal(long dealId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var deals = await db.BankDeal.Where(x => x.BankDealId >= dealId).ToListAsync();

            var deal = await db.BankDeal.Where(x => x.BankDealId == dealId).FirstOrDefaultAsync();

            var debit = deal.Debit;
            var credit = deal.Credit;

            foreach (var item in deals)
            {
                item.Balance += debit;
                item.Balance -= credit;

                db.BankDeal.Update(item);
            }

            if (deals.Count == 1)
            {
                var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();
                serial.BankSerial -= 1;
                db.SerialTable.Update(serial);
            }

            var cash = await db.CashInHand.Where(x => x.BankDealId == dealId).FirstOrDefaultAsync();

            if (cash != null)
            {
                db.CashInHand.Remove(cash);
            }

            db.BankDeal.Remove(deal);

            await db.SaveChangesAsync();

            return Json(" ریکارډ حذف شو");

        }

        public IActionResult BankRemainList()
        {
            return View();
        }

        public JsonResult TotalRemainBank()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;


            var af = (from d in db.Bank
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          address = d.Address,
                          phone = d.Phone,
                          balance = db.BankDeal.Where(x => x.CurrencyId == 1 && x.BankId == d.BankId).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefault()
                      }).ToList();
            var pk = (from d in db.Bank
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          address = d.Address,
                          phone = d.Phone,
                          balance = db.BankDeal.Where(x => x.CurrencyId == 2 && x.BankId == d.BankId).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var us = (from d in db.Bank
                      where d.DepartmentId == depId && d.Status == "Unlocked"
                      select new
                      {
                          name = d.Name,
                          address = d.Address,
                          phone = d.Phone,
                          balance = db.BankDeal.Where(x => x.CurrencyId == 3 && x.BankId == d.BankId).OrderByDescending(x => x.BankDealId).Select(x => x.Balance).FirstOrDefault()

                      }).ToList();
            var rec = new
            {
                af,
                pk,
                us,
            };
            return Json(rec);
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddBankDealerDealInfo(BankDealerViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var email = userManager.GetUserAsync(User).Result.Email;

                    var image = userManager.GetUserAsync(User).Result.ImagePath;

                    depId = userManager.GetUserAsync(User).Result.DepartmentId;

                    double cr = 0.0;
                    double cr2 = 0.0;
                    double deb = 0.0;
                    double deb2 = 0.0;

                    var balance2 = 0.0;
                    var balance = 0.0;

                    var type = "";

                    if (model.CustomerId != 0)
                    {

                        if (model.Type == "Debit")
                        {
                            deb = model.Amount;
                            deb2 = model.Amount2;

                            type = "FromBank";

                            balance = model.Amount;
                            balance2 = -model.Amount2;
                        }
                        else
                        {
                            cr = model.Amount;
                            cr2 = model.Amount2;

                            type = "ToBank";

                            balance = -model.Amount;
                            balance2 = model.Amount2;
                        }

                        var dealing = await db.CustomerDeal.Where(x =>
                        x.CustomerId == model.CustomerId
                        && x.CurrencyId == model.CurrencyId
                        && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();



                        if (dealing != null)
                        {
                            balance += dealing.Balance;
                        }


                        CustomerDeal deal = new CustomerDeal
                        {
                            //BillNo = bill,
                            Debit = deb,
                            Credit = cr,
                            Balance = balance,
                            CustomerId = model.CustomerId,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.CurrencyId,
                            Employee = email,
                            Type = model.Type,
                            Detail = model.Detail,
                            Image = image,
                            DepartmentId = depId,

                        };

                        await db.CustomerDeal.AddAsync(deal);
                        await db.SaveChangesAsync();

                        var dealing2 = await db.BankDeal.Where(x =>
                        x.BankId == model.BankId
                        && x.CurrencyId == model.Currency2Id
                        && x.DepartmentId == depId).OrderByDescending(x => x.BankDealId).FirstOrDefaultAsync();


                        if (dealing2 != null)
                        {
                            balance2 += dealing2.Balance;
                        }

                        var dealer = await db.Customer.FindAsync(model.CustomerId);
                        var curency = await db.Currency.FindAsync(model.CurrencyId);

                        var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                        var sno = serial.BankSerial ;

                        serial.BankSerial += 1;

                        db.SerialTable.Update(serial);

                        BankDeal bank = new BankDeal
                        {
                            Debit = deb2,
                            Credit = cr2,
                            Balance = balance2,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.Currency2Id,
                            BankId = model.BankId,
                            Employee = email,
                            Detail = model.Detail,
                            SerialNo = sno,
                            DealerName = dealer.Name,
                            Currency2 = curency.Currency1,
                            Amount = model.Amount,
                            Type = type,
                            DepartmentId = depId
                        };

                        await db.BankDeal.AddAsync(bank);

                        await db.SaveChangesAsync();

                        return Json("کهاتې معلومات ذخیره شول");
                    }
                    else
                    {

                        if (model.Type == "Debit")
                        {
                            deb = model.Amount;
                            deb2 = model.Amount2;

                            type = "FromBank";

                            balance = model.Amount;
                            balance2 = -model.Amount2;
                        }
                        else
                        {
                            cr = model.Amount;
                            cr2 = model.Amount2;

                            type = "ToBank";

                            balance = -model.Amount;
                            balance2 = model.Amount2;
                        }

                        var dealing = await db.DealerDeal.Where(x =>
                         x.DealerId == model.DealerId
                         && x.CurrencyId == model.CurrencyId
                         && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();


                        if (dealing != null)
                        {
                            balance += dealing.Balance;
                        }


                        DealerDeal deal = new DealerDeal
                        {
                            //BillNo = bill,
                            Debit = deb,
                            Credit = cr,
                            Balance = balance,
                            DealerId = model.DealerId,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.CurrencyId,
                            Employee = email,
                            Type = model.Type,
                            Detail = model.Detail,
                            Image = image,
                            DepartmentId = depId,

                        };

                        await db.DealerDeal.AddAsync(deal);

                        await db.SaveChangesAsync();

                        var dealing2 = await db.BankDeal.Where(x =>
                         x.BankId == model.BankId
                         && x.CurrencyId == model.Currency2Id
                         && x.DepartmentId == depId).OrderByDescending(x => x.BankDealId).FirstOrDefaultAsync();


                        if (dealing2 != null)
                        {
                            balance2 += dealing2.Balance;
                        }

                        var dealer = await db.Dealer.FindAsync(model.DealerId);
                        var curency = await db.Currency.FindAsync(model.CurrencyId);

                        var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                        var sno = serial.BankSerial;

                        serial.BankSerial += 1;

                        db.SerialTable.Update(serial);

                        BankDeal bank = new BankDeal
                        {
                            Debit = deb2,
                            Credit = cr2,
                            Balance = balance2,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.Currency2Id,
                            BankId = model.BankId,
                            Employee = email,
                            Detail = model.Detail,
                            SerialNo = sno,
                            DealerName = dealer.Name,
                            Currency2 = curency.Currency1,
                            Amount = model.Amount,
                            Type = type,
                            DepartmentId = depId
                        };

                        await db.BankDeal.AddAsync(bank);

                        await db.SaveChangesAsync();

                        return Json("کهاتې معلومات ذخیره شول");
                    }
                }
                catch (Exception e)
                {

                    return Json("اشتباه" + e.Message);
                }

            }

            return Json("اشتباه");

        }

        public IActionResult CashRemainList()
        {
            return View();
        }

        public JsonResult LoadCashInHand()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var af = (from d in db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 1)
                      select new
                      {
                          debit = d.Debit,
                          credit = d.Credit,
                          date = Convert.ToDateTime(d.Date).ToShortDateString(),
                          desc = d.Description ?? "",
                          currency = "افغانۍ"

                      }).ToList();

            var pk = (from d in db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 2)
                      select new
                      {
                          debit = d.Debit,
                          credit = d.Credit,
                          date = Convert.ToDateTime(d.Date).ToShortDateString(),
                          desc = d.Description ?? "",
                          currency = "کلدارې"

                      }).ToList();

            var us = (from d in db.CashInHand.Where(x => x.DepartmentId == depId && x.CurrencyId == 3)
                      select new
                      {
                          debit = d.Debit,
                          credit = d.Credit,
                          date = Convert.ToDateTime(d.Date).ToShortDateString(),
                          desc = d.Description ?? "",
                          currency = "ډالر"

                      }).ToList();

            return Json(new
            {
                af,
                pk,
                us
            });
        }

        //[Authorize(Roles = "Admin Department")]
        //public async Task<IActionResult> EditDeal(int dealidedit, double debite, double credite, string detailse)
        //{
        //    var email = userManager.GetUserAsync(User).Result.Email;
        //    var empId = db.Employee.Where(e => e.Email == email).Select(r => r.EmployeeId).FirstOrDefault();

        //    var rec = db.Deal.Where(x => x.DealId == dealidedit).FirstOrDefault();
        //    var rec1 = db.CashInHand.Where(x => x.DealId == dealidedit).FirstOrDefault();
        //    if (rec == null || rec1 == null)
        //    {
        //        return Json("اشتباه");
        //    }
        //    if (rec != null)
        //    {
        //        rec.Debit = debite;
        //        rec.Credit = credite;
        //        rec.Detail = detailse;
        //        rec.EmployeeId = empId;
        //        db.Deal.Update(rec);
        //        await db.SaveChangesAsync();
        //    }

        //    if (rec1 != null)
        //    {
        //        rec1.Debit = debite;
        //        rec1.Credit = credite;
        //        rec1.Description = detailse;
        //        db.CashInHand.Update(rec1);
        //        await db.SaveChangesAsync();
        //    }
        //    return Json("done");

        //    //db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //db.Entry(rec).Property(x => x.Debit).IsModified = true;
        //    //db.Entry(rec).Property(x => x.Credit).IsModified = true;
        //    //db.Entry(rec).Property(x => x.Detail).IsModified = true;

        //    //var str1 = "کهاته" + "," + rec.DealId;
        //    //var rec1 = db.CashInHand.Where(x => x.Description == str1).FirstOrDefault();
        //    //if (rec1 != null)
        //    //{
        //    //    rec1.Debit = debite;
        //    //    rec1.Credit = credite;
        //    //    db.Entry(rec1).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //    db.Entry(rec1).Property(x => x.Debit).IsModified = true;
        //    //    db.Entry(rec1).Property(x => x.Credit).IsModified = true;
        //    //    db.SaveChanges();
        //    //    return Json("done");
        //    //}

        //    //var str2 = "د اجناسو پلور" + "," + rec.DealId;

        //    //var rec2 = db.CashInHand.Where(x => x.Description == str2).FirstOrDefault();
        //    //if (rec2 != null)
        //    //{
        //    //    rec2.Credit = credite;
        //    //    db.Entry(rec2).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //    db.Entry(rec2).Property(x => x.Credit).IsModified = true;
        //    //    db.SaveChanges();
        //    //    return Json("done");

        //    //}
        //    //var str3 = "پیرل" + "," + rec.DealId;
        //    //var rec3 = db.CashInHand.Where(x => x.Description == str3).FirstOrDefault();
        //    //if (rec3 != null)
        //    //{
        //    //    rec3.Debit = debite;
        //    //    db.Entry(rec3).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    //    db.Entry(rec3).Property(x => x.Debit).IsModified = true;
        //    //    db.SaveChanges();
        //    //    return Json("done");
        //    //}

        //}

        //public JsonResult PurchaseDeal()
        //{
        //    var rec = db.Deal.Where(x => x.Dealer.DealerTypeId == 1 && x.Loan != "NO").ToList();
        //    return Json(rec);
        //}
        //public JsonResult CustomerDeal()
        //{
        //    var rec = db.Deal.Where(x => x.Dealer.DealerTypeId != 1 && x.Loan != "NO").ToList();
        //    return Json(rec);
        //}
        //public JsonResult Dealday()
        //{
        //    var day = DateTime.Now.DayOfWeek;
        //    var rec = db.Dealler.Where(x => x.PaymentDay == day.ToString() && x.DealerTypeId == 2).Count();
        //    return Json(rec);
        //}

        //public JsonResult TodaysDealsers()
        //{
        //    var day = DateTime.Now.DayOfWeek;
        //    var af = (from d in db.Dealler
        //              where d.PaymentDay == day.ToString() && d.DealerTypeId == 2
        //              select new
        //              {
        //                  name = d.DealerName,
        //                  fname = d.FatherName,
        //                  province = d.Province,
        //                  phone = d.Phone,
        //                  debit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 1).Sum(x => x.Debit),
        //                  credit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 1).Sum(x => x.Credit),
        //              }).ToList();
        //    var pk = (from d in db.Dealler
        //              where d.PaymentDay == day.ToString() && d.DealerTypeId == 2
        //              select new
        //              {
        //                  name = d.DealerName,
        //                  fname = d.FatherName,
        //                  province = d.Province,
        //                  phone = d.Phone,
        //                  debit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 2).Sum(x => x.Debit),
        //                  credit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 2).Sum(x => x.Credit),
        //              }).ToList();
        //    var us = (from d in db.Dealler
        //              where d.PaymentDay == day.ToString() && d.DealerTypeId == 2
        //              select new
        //              {
        //                  name = d.DealerName,
        //                  fname = d.FatherName,
        //                  province = d.Province,
        //                  phone = d.Phone,
        //                  debit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 3).Sum(x => x.Debit),
        //                  credit = db.Deal.Where(x => x.DealerId == d.DealerId && x.Loan != "NO" && x.CurrencyId == 3).Sum(x => x.Credit),
        //              }).ToList();
        //    var rec = new
        //    {
        //        af,
        //        pk,
        //        us,
        //    };
        //    return Json(rec);
        //}

        public IActionResult DealersList()
        {
            return View();
        }

        public async Task<JsonResult> FetchBankSerialNumber()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            long serialNumber = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).Select(x => x.BankSerial).FirstOrDefault();

            if (serialNumber == 0)
            {
                SerialTable serial = new SerialTable
                {
                    BankSerial = 1,
                    DepartmentId = depId,
                };

                db.SerialTable.Add(serial);
                await db.SaveChangesAsync();

                return Json(new
                {
                    serialNumber = 1,
                });
            }

            return Json(new
            {
                serialNumber = serialNumber,
            });
        }
    }
}

