using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;
using GeneralSalesDb.Models.ViewModels;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = ("Stock Department,Admin Department,Purchase Department"))]
    public class PurchaseController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();

        private readonly UserManager<ApplicationUser> userManager;

        int depId = 0;

        public PurchaseController(UserManager<ApplicationUser> userManager)
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

            var dealers = (from d in db.Dealer
                           where d.DepartmentId == depId && d.Status == "Unlocked"
                           select new SelectListItem()
                           {
                               Text = d.Name,
                               Value = d.DealerId.ToString()
                           }).ToList();
            ViewBag.dealers = dealers;

            var units = (from d in db.Unit
                         where d.DepartmentId == depId
                         select new SelectListItem()
                         {
                             Text = d.Unit1,
                             Value = d.UnitId.ToString()
                         }).ToList();
            ViewBag.units = units;

            var stock = (from d in db.Stock
                         where d.DepartmentId == depId
                         select new SelectListItem()
                         {
                             Text = d.Stock1,
                             Value = d.StockId.ToString()
                         }).ToList();
            ViewBag.stock = stock;

            return View();
        }

        public async Task<JsonResult> FetchSerialNumber()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            long serialNumber = (long)db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).Select(x => x.PurchaseSerial).FirstOrDefault();

            if (serialNumber == 0)
            {
                SerialTable serial = new SerialTable
                {
                    SerialNumberId = 1,
                    PurchaseSerial = 1,
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

        [HttpPost]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddPurchasedItems(string data, double credit, string details, double paid)
        {
            try
            {
                dynamic items = JsonConvert.DeserializeObject(data);

                var email = userManager.GetUserAsync(User).Result.Email;
                var image = userManager.GetUserAsync(User).Result.ImagePath;
                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                int dealerId = 0;
                int currencyId = 0;
                string bill = "";
                var totalQty = 0.0;
                double expense = 0.0;

                foreach (var item in items)
                {
                    int inUnit = item.inUnit;
                    double qty = item.qty;
                    double qty2 = item.qty2;
                    expense = item.expense;

                    totalQty += (qty2 / inUnit) + qty;
                }

                var unitExpense = expense / totalQty;

                var serial = db.SerialTable.Where(x => x.DepartmentId == depId && x.SerialNumberId == 1).FirstOrDefault();

                serial.PurchaseSerial += 1;

                db.SerialTable.Update(serial);

                foreach (var item in items)
                {
                    bill = item.bill;
                    dealerId = item.dealerId;
                    currencyId = item.currencyId;
                    int itemId = item.itemId;
                    int unitId = item.unitId;
                    int inUnit = item.inUnit;
                    double qty = item.qty;
                    double qty2 = item.qty2;
                    int stockId = item.stockId;
                    int shortage = item.shortage;
                    double price = item.price;

                    var dbtotalQty = (qty * inUnit) + qty2;

                    var foundItem = await db.StockItems.Where(x =>
                       x.ItemId == itemId
                    && x.CurrencyId == currencyId
                    && x.StockId == stockId
                    && x.Price == price + unitExpense
                    && x.UnitId == unitId
                    && x.InUnit == inUnit
                    && x.DepartmentId == depId).OrderByDescending(x => x.StockItemId).FirstOrDefaultAsync();

                    long StockItemId = 0;

                    if (foundItem != null)
                    {
                        foundItem.Quantity += dbtotalQty;

                        db.StockItems.Update(foundItem);

                        StockItemId = foundItem.StockItemId;
                    }
                    else
                    {
                        StockItems newItem = new StockItems
                        {
                            ItemId = itemId,
                            UnitId = unitId,
                            CurrencyId = currencyId,
                            Price = price + unitExpense,
                            InUnit = inUnit,
                            Quantity = dbtotalQty,
                            ShortageQty = shortage,
                            StockId = stockId,
                            Date = DateTime.Now.Date,
                            DepartmentId = depId,
                        };
                        await db.StockItems.AddAsync(newItem);

                        await db.SaveChangesAsync();

                        StockItemId = newItem.StockItemId;
                    }


                    Purchase purchase = new Purchase
                    {
                        BillNo = bill,
                        DealerId = dealerId,
                        CurrencyId = currencyId,
                        Employee = email,
                        ItemId = itemId,
                        UnitId = unitId,
                        InUnit = inUnit,
                        Quantity = qty,
                        Quantity2 = qty2,
                        Price = price,
                        StockId = stockId,
                        PurchaseDate = DateTime.Now.Date,
                        Status = null,
                        StockItemId = StockItemId,
                        Expense = unitExpense,
                        DepartmentId = depId,
                    };

                    await db.Purchase.AddAsync(purchase);

                }

                if (expense != 0)
                {
                    PurExpense pur = new PurExpense
                    {
                        BillNo = bill,
                        Amount = expense,
                        CurrencyId = currencyId,
                        Date = DateTime.Now.Date,
                        DealerId = dealerId,
                        Desc = details,
                    };

                    await db.PurExpense.AddAsync(pur);
                }

                var dealing = await db.DealerDeal.Where(x =>
                   x.DealerId == dealerId
                && x.CurrencyId == currencyId
                && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                var balance = credit;

                if (dealing != null)
                {
                    balance += dealing.Balance;
                }

                DealerDeal deal = new DealerDeal
                {
                    BillNo = bill,
                    Debit = 0,
                    Credit = credit,
                    Balance = balance,
                    DealerId = dealerId,
                    Date = DateTime.Now.Date,
                    CurrencyId = currencyId,
                    Employee = email,
                    Type = "Purchase",
                    Loan = "",
                    Detail = details,
                    DepartmentId = depId,

                };
                await db.DealerDeal.AddAsync(deal);
                await db.SaveChangesAsync();

                if (paid != 0)
                {
                    var dealing2 = await db.DealerDeal.Where(x =>
                    x.DealerId == dealerId
                    && x.CurrencyId == currencyId
                     && x.DepartmentId == depId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                    var balance2 = -paid;

                    if (dealing2 != null)
                    {
                        balance2 += dealing2.Balance;
                    }

                    DealerDeal deal2 = new DealerDeal
                    {
                        BillNo = bill,
                        Debit = paid,
                        Credit = 0,
                        Balance = balance2,
                        DealerId = dealerId,
                        Date = DateTime.Now.Date,
                        CurrencyId = currencyId,
                        Employee = email,
                        Type = "Purchase",
                        Loan = "Cash",
                        Detail = details,
                        DepartmentId = depId,

                    };
                    await db.DealerDeal.AddAsync(deal2);

                    await db.SaveChangesAsync();

                    CashInHand cash = new CashInHand
                    {
                        Debit = paid,
                        Credit = 0,
                        CurrencyId = currencyId,
                        Date = DateTime.Now.Date,
                        DealId = deal2.DealId,
                        DepartmentId = 1,
                        Description = details,
                    };

                    await db.CashInHand.AddAsync(cash);

                    await db.SaveChangesAsync();

                }

                return Json("success");
            }
            catch (Exception e)
            {

                return Json("error", e.Message);
            }
        }

        public JsonResult FetchPurchasedItems(DateTime fromDate, DateTime toDate)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            if (fromDate.Year != 0001 && toDate.Year != 0001)
            {
                var result = (from p in db.Purchase
                              where p.Status != "Returned" && p.PurchaseDate >= fromDate && p.PurchaseDate <= toDate && p.DepartmentId == depId
                              join item in db.Item on p.ItemId equals item.ItemId
                              join de in db.Dealer on p.DealerId equals de.DealerId
                              join u in db.Unit on p.UnitId equals u.UnitId
                              join currenc in db.Currency on p.CurrencyId equals currenc.CurrencyId
                              select new
                              {
                                  purchaseid = p.PurchaseId,
                                  billno = p.BillNo,
                                  itemid = p.ItemId,
                                  unit = u.Unit1,
                                  inunit = p.InUnit,
                                  item = item.Name,
                                  dealerid = p.DealerId,
                                  dealer = de.Name,
                                  qty = p.Quantity,
                                  qty2 = p.Quantity2,
                                  price = p.Price,
                                  expense = p.Expense,
                                  stock = p.Stock.Stock1,
                                  currencyid = p.CurrencyId,
                                  currency = currenc.Currency1,
                                  employee = db.Employee.Where(x => x.Email == p.Employee).Select(x => x.Name).FirstOrDefault(),
                                  date = Convert.ToDateTime(p.PurchaseDate).ToShortDateString(),

                              }).ToList().OrderByDescending(x => x.purchaseid);

                return Json(result);
            }
            else
            {
                var result = (from p in db.Purchase

                              where p.Status != "Returned"
                              && p.PurchaseDate.Value.Year == DateTime.Now.Year
                              && p.PurchaseDate.Value.Month == DateTime.Now.Month
                              && p.DepartmentId == depId
                              join item in db.Item on p.ItemId equals item.ItemId
                              join de in db.Dealer on p.DealerId equals de.DealerId
                              join u in db.Unit on p.UnitId equals u.UnitId
                              join currenc in db.Currency on p.CurrencyId equals currenc.CurrencyId
                              select new
                              {
                                  purchaseid = p.PurchaseId,
                                  billno = p.BillNo,
                                  itemid = p.ItemId,
                                  unit = u.Unit1,
                                  inunit = p.InUnit,
                                  item = item.Name,
                                  dealerid = p.DealerId,
                                  dealer = de.Name,
                                  qty = p.Quantity,
                                  qty2 = p.Quantity2,
                                  price = p.Price,
                                  expense = p.Expense,
                                  stock = p.Stock.Stock1,
                                  currencyid = p.CurrencyId,
                                  currency = currenc.Currency1,
                                  employee = db.Employee.Where(x => x.Email == p.Employee).Select(x => x.Name).FirstOrDefault(),
                                  date = Convert.ToDateTime(p.PurchaseDate).ToShortDateString(),

                              }).ToList().OrderByDescending(x => x.purchaseid);

                return Json(result);
            }
        }

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ReturnItems(long purId, int dealerIdd, int inUnitt, double ReturnQty, string Details)
        {
            if (ReturnQty != 0)
            {
                var email = userManager.GetUserAsync(User).Result.Email;

                var qty = inUnitt * ReturnQty;

                var pur = await db.Purchase.FindAsync(purId);

                var stock = await db.StockItems.FindAsync(pur.StockItemId);

                var purQty = pur.Quantity + (pur.Quantity2 / pur.InUnit);

                if (purQty == ReturnQty)
                {
                    pur.Status = "Returned";

                    pur.ReturnedDate = DateTime.Now.Date;

                    pur.Desc = Details;

                    db.Purchase.Update(pur);

                }
                else
                {

                    var omda = Math.Floor(ReturnQty);
                    var parchon = Math.Ceiling(ReturnQty);

                    pur.Quantity -= omda;
                    pur.Quantity2 -= parchon;

                    db.Purchase.Update(pur);

                    Purchase purchase = new Purchase
                    {
                        BillNo = pur.BillNo,
                        DealerId = pur.DealerId,
                        CurrencyId = pur.CurrencyId,
                        Employee = email,
                        ItemId = pur.ItemId,
                        UnitId = pur.UnitId,
                        InUnit = pur.InUnit,
                        Quantity = omda,
                        Quantity2 = parchon,
                        Price = pur.Price,
                        Expense = pur.Expense,
                        Status = "Returned",
                        ReturnedDate = DateTime.Now.Date,
                        PurchaseDate = DateTime.Now.Date,
                        Desc = Details,
                        DepartmentId = pur.DepartmentId,
                    };

                    await db.Purchase.AddAsync(purchase);

                }

                if (stock != null)
                {
                    stock.Quantity -= qty;

                    db.StockItems.Update(stock);
                }


                var dealing = await db.DealerDeal.Where(x =>
                x.DealerId == pur.DealerId &&
                x.CurrencyId == pur.CurrencyId &&
                x.DepartmentId == pur.DepartmentId).OrderByDescending(x => x.DealId).FirstOrDefaultAsync();

                var balance = ReturnQty * pur.Price;

                if (dealing != null)
                {
                    balance = dealing.Balance - balance;
                }

                DealerDeal deal = new DealerDeal
                {
                    BillNo = pur.BillNo,
                    Credit = 0,
                    Debit = ReturnQty * pur.Price,
                    Date = DateTime.Now.Date,
                    Balance = balance,
                    Detail = Details,
                    CurrencyId = pur.CurrencyId,
                    Employee = email,
                    Type = "Returned",
                    DealerId = pur.DealerId,
                    DepartmentId = pur.DepartmentId,
                };

                await db.DealerDeal.AddAsync(deal);

                await db.SaveChangesAsync();

                return Json("جنس واپس شو");
            }

            return Json("وزن باید صفر نه وی");
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteBill(int dealerId, string billNumber)
        {
            if (dealerId != 0 && billNumber != null)
            {

                var pur = await db.Purchase.Where(x => x.DealerId == dealerId && x.BillNo == billNumber).ToListAsync();

                var purExp = await db.PurExpense.Where(x => x.DealerId == dealerId && x.BillNo == billNumber).FirstOrDefaultAsync();

                if (pur.Count == 0)
                {
                    return Json("مهربانی له مخې بیل نمبر او کهاتدار چیک کړئ");
                }

                if (purExp != null)
                {
                    db.PurExpense.Remove(purExp);
                }


                var deal = await db.DealerDeal.Where(x => x.DealerId == dealerId && x.BillNo == billNumber && x.CurrencyId==pur.FirstOrDefault().CurrencyId).FirstOrDefaultAsync();

                if (deal != null)
                {

                    var deals = await db.DealerDeal.Where(x => x.DealId >= deal.DealId && x.DealerId== deal.DealerId && x.CurrencyId==deal.CurrencyId).ToListAsync();

                    var debit = deal.Debit;
                    var credit = deal.Credit;

                    foreach (var item in deals)
                    {
                        item.Balance += debit;
                        item.Balance -= credit;

                        db.DealerDeal.Update(item);
                    }

                    if (deals.Count == 1 || deals.Count == 2)
                    {
                        var serial = db.SerialTable.Where(x => x.SerialNumberId == 1).FirstOrDefault();

                        serial.PurchaseSerial -= 1;

                        db.SerialTable.Update(serial);
                    }

                    db.DealerDeal.Remove(deal);

                    await db.SaveChangesAsync();
                }

                var deal2 = await db.DealerDeal.Where(x => x.DealerId == dealerId && x.BillNo == billNumber && x.CurrencyId == pur.FirstOrDefault().CurrencyId).FirstOrDefaultAsync();

                if (deal2 != null)
                {


                    var deals2 = await db.DealerDeal.Where(x => x.DealId >= deal.DealId && x.DealerId==deal2.DealerId && x.CurrencyId==deal2.DealerId).ToListAsync();

                    var debit2 = deal2.Debit;
                    var credit2 = deal2.Credit;

                    foreach (var item in deals2)
                    {
                        item.Balance -= debit2;
                        item.Balance += credit2;

                        db.DealerDeal.Update(item);
                    }

                    var cash = db.CashInHand.Where(x => x.DealId == deal2.DealId && x.CurrencyId==deal2.CurrencyId).FirstOrDefault();
                    if (cash != null)
                    {
                        db.CashInHand.Remove(cash);
                    }

                    db.DealerDeal.Remove(deal2);
                    await db.SaveChangesAsync();
                }


                foreach (var item in pur)
                {
                    var stock = await db.StockItems.Where(x => x.StockItemId == item.StockItemId).FirstOrDefaultAsync();

                    stock.Quantity -= (item.Quantity * item.InUnit) + item.Quantity2;

                    db.StockItems.Update(stock);

                    db.Purchase.Remove(item);
                }

                await db.SaveChangesAsync();

                return Json("بیل نمبر اجناس مکمل حذف شو");
            }
            return Json("مهربانی له مخې بیل نمبر او کهاتدار چیک کړئ");
        }

        public IActionResult ReturnedStockItems()
        {
            return View();
        }

        public JsonResult FetchPurchaseReturned()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var List = (from p in db.Purchase.Where(x => x.Status == "Returned")
                        join item in db.Item on p.ItemId equals item.ItemId
                        join de in db.Dealer on p.DealerId equals de.DealerId
                        join currenc in db.Currency on p.CurrencyId equals currenc.CurrencyId
                        where p.DepartmentId == depId
                        select new
                        {
                            purchaseid = p.PurchaseId,
                            billno = p.BillNo,
                            itemid = p.ItemId,
                            item = item.Name,
                            dealerid = p.DealerId,
                            dealer = de.Name,
                            price = p.Price,
                            expense = p.Expense,
                            quantity = p.Quantity,
                            quantity2 = p.Quantity2,
                            currencyid = p.CurrencyId,
                            currency = currenc.Currency1,
                            purchasestate = p.Status,
                            unit = p.Unit.Unit1,
                            inUnit = p.InUnit,
                            returndate = Convert.ToDateTime(p.ReturnedDate).ToShortDateString(),
                            desc = p.Desc ?? "",
                        }).ToList().OrderByDescending(x => x.purchaseid);
            return Json(List);
        }

        public IActionResult Stock()
        {
            var stock = (from d in db.Stock
                         select new SelectListItem()
                         {
                             Text = d.Stock1,
                             Value = d.StockId.ToString()
                         }).ToList();
            ViewBag.stock = stock;
            return View();
        }

        public async Task<IActionResult> MoveToAnotherStock(long item2idstock, int quantitystock, int oldstock, int newstock)
        {
            var item2 = db.StockItems.Where(x => x.StockItemId == item2idstock && x.StockId == oldstock).FirstOrDefault();
            if (item2 != null)
            {

                var item3 = db.StockItems.Where(x => x.StockItemId == item2idstock && x.StockId == newstock && x.Price == item2.Price && x.InUnit == item2.InUnit && x.CurrencyId == item2.CurrencyId).FirstOrDefault();

                if (item3 != null)
                {
                    item3.Quantity += item3.InUnit * quantitystock;
                    db.StockItems.Update(item3);
                }
                else
                {
                    StockItems it = new StockItems
                    {
                        ItemId = item2.ItemId,
                        UnitId = item2.UnitId,
                        CurrencyId = item2.CurrencyId,
                        Price = item2.Price,
                        InUnit = item2.InUnit,
                        Quantity = item2.InUnit * quantitystock,
                        ShortageQty = item2.ShortageQty,
                        StockId = newstock,
                        Type = item2.Type,
                        Date = DateTime.Now.Date,
                        DepartmentId = 1,
                    };
                    db.StockItems.Add(it);
                }

                item2.Quantity -= item2.InUnit * quantitystock;
                db.StockItems.Update(item2);
                await db.SaveChangesAsync();
                return Json("جنس بل ګودام ته انتقال شو");
            }

            return Json("error");
        }

        public async Task<JsonResult> FetchStockItems()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var stocks = await (from i in db.StockItems
                                join curr in db.Currency on i.CurrencyId equals curr.CurrencyId
                                join unit in db.Unit on i.UnitId equals unit.UnitId
                                where i.Quantity > 0 && i.DepartmentId == depId
                                select new
                                {
                                    stockitemid = i.StockItemId,
                                    itemid = i.ItemId,
                                    itemname = i.Item.Name,
                                    currency = curr.Currency1,
                                    price = i.Price,
                                    quantity = i.Quantity,
                                    currencyid = i.CurrencyId,
                                    inUnit = i.InUnit,
                                    unitid = i.UnitId,
                                    unit = unit.Unit1,
                                    stock = i.Stock.Stock1,
                                    stockid = i.StockId,
                                }).OrderByDescending(x => x.stockitemid).ToListAsync();

            return Json(stocks);
        }

        public JsonResult FetchItemInfo(int itemId)
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var result = (from i in db.Item
                          join r in db.StockItems on i.ItemId equals r.ItemId
                          where i.ItemId == itemId
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
                          }).OrderByDescending(x => x.stockItemId).Take(1).ToList();

            return Json(result);

        }
    }
}
