using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Sale Department,Admin Department")]
    public class SaleController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();

        private readonly UserManager<ApplicationUser> userManager;

        int depId = 0;
        public SaleController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var items = (from d in db.Item
                         where d.DepartmentId == depId
                         select new SelectListItem()
                         {
                             Text = d.Name,
                             Value = d.ItemId.ToString()
                         }).ToList();
            ViewBag.items = items;

            var customers = (from d in db.Customer
                             where d.DepartmentId == depId
                             select new SelectListItem()
                             {
                                 Text = d.CustomerId + " - " + d.Name,
                                 Value = d.CustomerId.ToString()
                             }).ToList();
            ViewBag.customers = customers;

            var unit = (from d in db.Unit
                        where d.DepartmentId == depId
                        select new SelectListItem()
                        {
                            Text = d.Unit1,
                            Value = d.UnitId.ToString()
                        }).ToList();
            ViewBag.unit = unit;


            var stock = (from d in db.Stock
                        where d.DepartmentId == depId
                        select new SelectListItem()
                        {
                            Text = d.Stock1 ,
                            Value = d.StockId.ToString()
                        }).ToList();
            ViewBag.stock = stock;

            return View();
        }

        public async Task<JsonResult> FetchSerialNumber()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            long serialNumber = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).Select(x => x.SaleSerial).FirstOrDefault();

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

        public JsonResult FetchLastRemain(int customerId, int currencyId)
        {
            var result = db.CustomerDeal.Where(x => x.CustomerId == customerId && x.CurrencyId == currencyId).OrderByDescending(x => x.DealId).Select(x => x.Balance).FirstOrDefault();


            return Json(result);

        }

        public JsonResult FetchItemInfo(int itemId,int stockId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var result = (from i in db.Item
                          join r in db.StockItems on i.ItemId equals r.ItemId
                          where i.ItemId == itemId && r.StockId==stockId && r.Quantity > 0
                          select new
                          {
                              stockItemId = r.StockItemId,
                              itemid = i.ItemId,
                              itemname = i.Name,
                              quantity = r.Quantity,
                              price = r.Price,
                              currency = r.Currency.Currency1,
                              currencyid = r.CurrencyId,
                              inunit = r.InUnit,
                              unitid = r.UnitId,
                          }).OrderByDescending(x => x.stockItemId).Take(5).ToList();

            return Json(result);

        }

        [HttpPost]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddSaledItems(string data, string customerInfo)
        {
            try
            {
                dynamic items = JsonConvert.DeserializeObject(data);

                dynamic info = JsonConvert.DeserializeObject(customerInfo);

                var email = userManager.GetUserAsync(User).Result.Email;
                var image = userManager.GetUserAsync(User).Result.ImagePath;

                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                string serial = info.serial;
                int customerid = info.customerid;
                int currencyid = info.currencyid;
                int stockid = info.stockid;
                double total = info.total;
                double loading = info.loading;
                double prev = info.prev;
                double paid = info.paid;
                string desc = info.desc;

                foreach (var item in items)
                {
                    long stockItemId = item.stockItemId;
                    int itemid = item.itemid;
                    int unitid = item.unitid;
                    string type = item.type;
                    double purprice = item.purprice;
                    double saleprice = item.saleprice;
                    double qty = item.qty;
                    double discount = item.discount;

                    double stockQty = qty * 1000;



                    var stockItem = await db.StockItems.FindAsync(stockItemId);

                    var inUnit = 1;
                    if (type == "عمده")
                    {
                        inUnit = stockItem.InUnit;
                    }

                    if (stockItem != null)
                    {
                        stockItem.Quantity -= qty * inUnit;

                        db.StockItems.Update(stockItem);
                    }


                    Sale sale = new Sale
                    {
                        BillNo = serial,
                        CustomerId = customerid,
                        ItemId = itemid,
                        CurrencyId = currencyid,
                        PurchasePrice = purprice,
                        SalePrice = saleprice,
                        Quantity = qty,
                        Discount = discount,
                        SaleDate = DateTime.Now.Date,
                        Employe = email,
                        Note = desc,
                        UnitId = unitid,
                        InUnit = inUnit,
                        StockId = stockid,
                        StockItemId = stockItemId,
                        SaleType = type,
                        DepartmentId = depId,
                    };
                    await db.Sale.AddAsync(sale);

                    await db.SaveChangesAsync();


                }

                var serialNumber = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                serialNumber.SaleSerial += 1;

                db.SerialTable.Update(serialNumber);

                PrintTable print = new PrintTable
                {
                    BillNo = serial,
                    CustomerId = customerid,
                    StockId = stockid,
                    CurrencyId = currencyid,
                    TotalBill = total,
                    Loading = loading,
                    Previous = prev,
                    Paid = paid,
                    Date = DateTime.Now.Date,
                    Employe = email,
                    Desc = desc,
                    DepartmentId = depId
                };

                await db.PrintTable.AddAsync(print);

                await db.SaveChangesAsync();

                var dealing = await db.CustomerDeal.Where(x =>
                               x.CustomerId == customerid &&
                               x.CurrencyId == currencyid &&
                               x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                total += loading;

                var balance = total;

                if (dealing != null)
                {
                    balance += dealing.Balance;
                }

                long ser = (long)Convert.ToDouble(serial);

                CustomerDeal deal = new CustomerDeal
                {
                    BillNo = serial,
                    SerialNumber = ser,
                    Credit = 0,
                    Debit = total,
                    Balance = balance,
                    Date = DateTime.Now.Date,
                    Detail = desc,
                    CurrencyId = currencyid,
                    Employee = email,
                    CustomerId = customerid,
                    PrintId = print.PrintId,
                    Type = "Sale",
                    Loan = "",
                    DepartmentId = depId,
                };

                await db.CustomerDeal.AddAsync(deal);

                await db.SaveChangesAsync();

                if (paid != 0)
                {

                    var payment = await db.CustomerDeal.Where(x =>
                                  x.CustomerId == customerid &&
                                  x.CurrencyId == currencyid &&
                                  x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                    var balance2 = -paid;

                    if (payment != null)
                    {
                        balance2 += payment.Balance;
                    }

                    CustomerDeal deal2 = new CustomerDeal
                    {
                        BillNo = serial,
                        SerialNumber = ser,
                        Credit = paid,
                        Debit = 0,
                        Balance = balance2,
                        Date = DateTime.Now.Date,
                        Detail = desc,
                        CurrencyId = currencyid,
                        Employee = email,
                        CustomerId = customerid,
                        PrintId = print.PrintId,
                        Type = "Sale",
                        Loan = "Cash",
                        DepartmentId = depId,
                    };

                    await db.CustomerDeal.AddAsync(deal2);

                    await db.SaveChangesAsync();

                    CashInHand cash = new CashInHand
                    {
                        Debit = 0,
                        Credit = paid,
                        CurrencyId = currencyid,
                        Date = DateTime.Now.Date,
                        PrintCashId = print.PrintId,
                        DepartmentId = 1,
                        Description = desc,
                    };

                    await db.CashInHand.AddAsync(cash);

                    await db.SaveChangesAsync();
                }

                return Json("success");
            }
            catch (Exception e)
            {

                return Json("اشتباه", e.Message);
            }
        }

        public JsonResult FetchSaledBills(DateTime fromDate, DateTime toDate)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {
                var result = (from p in db.PrintTable
                              where p.DepartmentId == depId && p.Date >= fromDate && p.Date <= toDate
                              select new
                              {
                                  id = p.PrintId,
                                  bill = p.BillNo,
                                  customer = p.Customer.Name,
                                  cphone = p.Customer.Phone,
                                  caddress = p.Customer.Address,
                                  customerid = p.CustomerId,
                                  currency = p.Currency.Currency1,
                                  total = p.TotalBill,
                                  loading = p.Loading,
                                  prev = p.Previous,
                                  paid = p.Paid,
                                  stockId = p.StockId,
                                  stock = p.Stock.Stock1,
                                  employee = db.Employee.Where(x => x.Email == p.Employe).Select(x => x.Name).FirstOrDefault(),
                                  date = Convert.ToDateTime(p.Date).ToShortDateString(),
                                  desc = p.Desc ?? ""
                              }).OrderByDescending(x => x.id).ToList();

                return Json(result);
            }
            else
            {

                var result = (from p in db.PrintTable
                              where p.DepartmentId == depId
                              && p.Date.Value.Year == DateTime.Now.Year
                             && p.Date.Value.Month == DateTime.Now.Month
                              select new
                              {
                                  id = p.PrintId,
                                  bill = p.BillNo,
                                  customer = p.Customer.Name,
                                  cphone = p.Customer.Phone,
                                  caddress = p.Customer.Address,
                                  customerid = p.CustomerId,
                                  currency = p.Currency.Currency1,
                                  total = p.TotalBill,
                                  loading = p.Loading,
                                  prev = p.Previous,
                                  paid = p.Paid,
                                  stockId = p.StockId,
                                  stock = p.Stock.Stock1,
                                  employee = db.Employee.Where(x => x.Email == p.Employe).Select(x => x.Name).FirstOrDefault(),
                                  date = Convert.ToDateTime(p.Date).ToShortDateString(),
                                  desc = p.Desc ?? ""
                              }).OrderByDescending(x => x.id).ToList();

                return Json(result);
            }
        }

        public JsonResult FetchBillItems(int customerId, string bill)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var result = (from p in db.Sale
                          where p.CustomerId == customerId && p.BillNo == bill && p.DepartmentId == depId && p.SaleState != "Returned" 
                          select new
                          {
                              item = p.Item.Name,
                              unit = p.Unit.Unit1,
                              qty = p.Quantity,
                              bundle = p.Bundle,
                              saleprice = p.SalePrice,
                              discount = p.Discount,
                          }).ToList();

            return Json(result);

        }

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ReturnBill(long id, int customerId, string bill, DateTime date)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            var email = userManager.GetUserAsync(User).Result.Email;
            var image = userManager.GetUserAsync(User).Result.ImagePath;

            var print = await db.PrintTable.FindAsync(id);

            var items = await db.Sale.Where(x => x.CustomerId == customerId && x.BillNo == bill && x.SaleDate == date && x.DepartmentId == depId).ToListAsync();

            if (print == null || items.Count == 0)
            {
                return Json("اشتباه");
            }

            foreach (var item in items)
            {
                var stockItem = await db.StockItems.FindAsync(item.StockItemId);

                stockItem.Quantity += item.Quantity * item.InUnit;

                db.StockItems.Update(stockItem);

                item.SaleState = "Returned";

                item.SaleDate = DateTime.Now.Date;

                db.Sale.Update(item);
            }

            var dealing = await db.CustomerDeal.Where(x =>
                             x.CustomerId == customerId &&
                             x.CurrencyId == print.CurrencyId &&
                             x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();


            double balance = (double)-print.TotalBill;

            if (dealing != null)
            {
                balance += dealing.Balance;
            }

            long ser = (long)Convert.ToDouble(print.BillNo);

            CustomerDeal deal = new CustomerDeal
            {
                BillNo = print.BillNo,
                SerialNumber = ser,
                Credit = (double)print.TotalBill,
                Debit = 0,
                Balance = balance,
                Date = DateTime.Now.Date,
                Detail = "د بیل واپسې",
                CurrencyId = print.CurrencyId,
                Employee = email,
                CustomerId = customerId,
                Image = image,
                DepartmentId = depId,
            };

            await db.CustomerDeal.AddAsync(deal);

            db.PrintTable.Remove(print);

            await db.SaveChangesAsync();

            return Json("بیل مکمل واپس شو");
        }

        public JsonResult FetchSaledItems(DateTime fromDate, DateTime toDate)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;
            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {

                var result = (from p in db.Sale
                              where p.DepartmentId == depId && p.SaleState != "Returned" && p.SaleDate >= fromDate && p.SaleDate <= toDate
                              select new
                              {
                                  id = p.SaleId,
                                  bill = p.BillNo,
                                  customer = p.Customer.Name,
                                  customerid = p.CustomerId,
                                  item = p.Item.Name,
                                  unit = p.Unit.Unit1,
                                  inunit = p.InUnit,
                                  qty = p.Quantity,
                                  price = p.PurchasePrice,
                                  saleprice = p.SalePrice,
                                  discount = p.Discount,
                                  currency = p.Currency.Currency1,
                                  date = Convert.ToDateTime(p.SaleDate).ToShortDateString(),
                                  employee = db.Employee.Where(x => x.Email == p.Employe).Select(x => x.Name).FirstOrDefault()
                              }).ToList();

                return Json(result);
            }
            else
            {

                var result = (from p in db.Sale
                              where p.DepartmentId == depId && p.SaleState != "Returned" && p.SaleDate.Value.Year == DateTime.Now.Year
                             && p.SaleDate.Value.Month == DateTime.Now.Month
                              select new
                              {
                                  id = p.SaleId,
                                  bill = p.BillNo,
                                  customer = p.Customer.Name,
                                  customerid = p.CustomerId,
                                  item = p.Item.Name,
                                  unit = p.Unit.Unit1,
                                  inunit = p.InUnit,
                                  qty = p.Quantity,
                                  price = p.PurchasePrice,
                                  saleprice = p.SalePrice,
                                  discount = p.Discount,
                                  currency = p.Currency.Currency1,
                                  date = Convert.ToDateTime(p.SaleDate).ToShortDateString(),
                                  employee = db.Employee.Where(x => x.Email == p.Employe).Select(x => x.Name).FirstOrDefault()
                              }).ToList();

                return Json(result);
            }
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteSaledItem(long printId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var print = await db.PrintTable.Where(x => x.DepartmentId == depId && x.PrintId == printId).FirstOrDefaultAsync();

            var items = await db.Sale.Where(x => x.CustomerId == print.CustomerId && x.BillNo == print.BillNo && x.SaleDate == print.Date && x.DepartmentId == depId).ToListAsync();

            if (print == null || items.Count == 0)
            {
                return Json("اشتباه");
            }

            foreach (var item in items)
            {
                var stockItem = await db.StockItems.FindAsync(item.StockItemId);

                stockItem.Quantity += item.Quantity * stockItem.InUnit;

                db.StockItems.Update(stockItem);

                db.Sale.Remove(item);
            }

            var printcount = await db.PrintTable.Where(x => x.DepartmentId == depId && x.PrintId >= printId).ToListAsync();

            if (printcount.Count == 1)
            {
                var serialNumber = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                serialNumber.SaleSerial -= 1;

                db.SerialTable.Update(serialNumber);
            }

            var deal = await db.CustomerDeal.Where(x => x.DepartmentId == depId && x.PrintId == printId).FirstOrDefaultAsync();

            if (deal != null)
            {

            var deals = await db.CustomerDeal.Where(x => x.DealId >= deal.DealId && x.CustomerId == deal.CustomerId && x.CurrencyId==deal.CurrencyId).ToListAsync();


            var debit = deal.Debit;
            var credit = deal.Credit;

            foreach (var item in deals)
            {
                item.Balance -= debit;
                //item.Balance += credit;

                db.CustomerDeal.Update(item);
            }

            db.CustomerDeal.Remove(deal);

            await db.SaveChangesAsync();

            }

            var deal2 = await db.CustomerDeal.Where(x => x.DepartmentId == depId && x.PrintId == printId).FirstOrDefaultAsync();

            if (deal2 != null)
            {

                var deals2 = await db.CustomerDeal.Where(x => x.DealId >= deal.DealId && x.CustomerId==deal2.CustomerId && x.CurrencyId== deal2.CurrencyId).ToListAsync();


                var debit2 = deal2.Debit;
                var credit2 = deal2.Credit;

                foreach (var item in deals2)
                {
                    //item.Balance += debit2;
                    item.Balance += credit2;

                    db.CustomerDeal.Update(item);
                }

                db.CustomerDeal.Remove(deal2);

                var cash = db.CashInHand.Where(x => x.PrintCashId == printId).FirstOrDefault();

                if (cash != null)
                {
                    db.CashInHand.Remove(cash);
                }

            }

            db.PrintTable.Remove(print);

            await db.SaveChangesAsync();

            return Json(" ریکارد مکمل حذف شو");


        }

        public IActionResult ReturnedItems()
        {
            return View();
        }

        public JsonResult FetchSaledReturned()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var List = (from p in db.Sale.Where(x => x.SaleState == "Returned" && x.DepartmentId == depId)
                        select new
                        {
                            id = p.SaleId,
                            bill = p.BillNo,
                            item = p.Item.Name,
                            unit = p.Unit.Unit1,
                            inunit = p.InUnit,
                            customer = p.Customer.Name,
                            qty = p.Quantity,
                            price = p.SalePrice,
                            discount = p.Discount,
                            currency = p.Currency.Currency1,
                            p.SaleState,
                            date = Convert.ToDateTime(p.SaleDate).ToShortDateString(),
                        }).OrderByDescending(x => x.id).ToList();
            return Json(List);
        }

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ReturnItems(long purId, int dealerIdd, int inUnitt, double ReturnQty, string Details)
        {
            if (ReturnQty != 0)
            {
                var email = userManager.GetUserAsync(User).Result.Email;

                var qty = inUnitt * ReturnQty;

                var pur = await db.Sale.FindAsync(purId);

                var stock = await db.StockItems.FindAsync(pur.StockItemId);


                if (pur.Quantity == ReturnQty)
                {
                    pur.SaleState = "Returned";

                    pur.SaleDate = DateTime.Now.Date;

                    db.Sale.Update(pur);

                }
                else
                {
                    pur.Quantity -= ReturnQty;

                    db.Sale.Update(pur);

                    Sale purchase = new Sale
                    {
                        BillNo = pur.BillNo,
                        CustomerId = pur.CustomerId,
                        CurrencyId = pur.CurrencyId,
                        Employe = email,
                        ItemId = pur.ItemId,
                        UnitId = pur.UnitId,
                        InUnit = pur.InUnit,
                        Quantity = ReturnQty,
                        PurchasePrice = pur.PurchasePrice,
                        SalePrice = pur.SalePrice,
                        SaleState = "Returned",
                        SaleDate = DateTime.Now.Date,
                        DepartmentId = pur.DepartmentId,
                    };

                    await db.Sale.AddAsync(purchase);

                }

                if (stock != null)
                {
                    stock.Quantity += qty;

                    db.StockItems.Update(stock);
                }


                var dealing = await db.CustomerDeal.Where(x =>
                x.CustomerId == pur.CustomerId &&
                x.CurrencyId == pur.CurrencyId &&
                x.DepartmentId == pur.DepartmentId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                var balance = ReturnQty * pur.SalePrice;

                if (dealing != null)
                {
                    balance = dealing.Balance - balance;
                }

                CustomerDeal deal = new CustomerDeal
                {
                    BillNo = pur.BillNo,
                    Credit = 0,
                    Debit = ReturnQty * pur.SalePrice,
                    Date = DateTime.Now.Date,
                    Balance = balance,
                    Detail = Details,
                    CurrencyId = pur.CurrencyId,
                    Employee = email,
                    CustomerId = pur.CustomerId,
                    DepartmentId = pur.DepartmentId,
                };

                await db.CustomerDeal.AddAsync(deal);

                await db.SaveChangesAsync();

                return Json("جنس واپس شو");
            }

            return Json("وزن باید صفر نه وی");
        }
    }
}
