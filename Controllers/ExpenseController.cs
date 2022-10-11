using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GeneralSalesDb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models;
using Microsoft.AspNetCore.Identity;
using GeneralSalesDb.Models.Model;

namespace GeneralSalesDb.Controllers
{

    [Authorize(Roles = "Admin Department,Finance Department")]
    public class ExpenseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        int depId = 0;
        public ExpenseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }
        public IActionResult Expense()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var c = (from d in db.ExpenceType          
                     where d.DepartmentId ==depId
                     select new SelectListItem()
                     {
                         Text = d.ExpenceType1,
                         Value = d.ExpenceTypeId.ToString()
                     }).ToList();

            ViewBag.ExpenseType = c;
            return View();
        }

        public JsonResult LoadExpenseType()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = db.ExpenceType.Where(d=>d.DepartmentId==depId ).OrderByDescending(d => d.ExpenceTypeId);
            return Json(rec);
        }

        public JsonResult LoadExpense()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var rec = (from i in db.Expence
                       join c in db.ExpenceType on i.ExpenceTypeId equals c.ExpenceTypeId
                       where i.DepartmentId == depId
                       select new
                       {
                           expenceId = i.ExpenceId,
                           expenceAmount = i.ExpenceAmount,
                           currency = i.Currency.Currency1,
                           currencyId = i.CurrencyId,
                           expenceType = c.ExpenceType1,
                           expenceDate = Convert.ToDateTime(i.ExpenceDate),
                           expenceDateShow = Convert.ToDateTime(i.ExpenceDate).ToShortDateString(),
                           detail = i.Detail ?? "",
                           expenceTypeId = i.ExpenceTypeId

                       }).ToList().OrderByDescending(d => d.expenceId);
            return Json(rec);
        }

        public IActionResult IsExpenseTypeExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            if (model.expenseType.ExpenseTypeId != 0)
            {
                return Json(true);
            }
            var rec = db.ExpenceType.Any(d => d.ExpenceType1 == model.expenseType.ExpenseType &&d.DepartmentId==depId);

            if (!rec)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> SaveExpenseType(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.expenseType.ExpenseTypeId != 0)
                {
                    var rec = await db.ExpenceType.FindAsync(model.expenseType.ExpenseTypeId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.ExpenceType1 = model.expenseType.ExpenseType;


                    db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.Entry(rec).Property(d => d.ExpenceType1).IsModified = true;

                    await db.SaveChangesAsync();

                    return Json("د مصرف نوع  تغیر شوه.");
                }
                else
                {
                    ExpenceType c = new ExpenceType()
                    {
                        ExpenceType1 = model.expenseType.ExpenseType,
                        DepartmentId =depId
                    };
                    db.ExpenceType.Add(c);
                    await db.SaveChangesAsync();
                    return Json("د مصرف نوع ذخیره شوه.");
                }

            }
            return View("Error");
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> SaveExpense(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.expense.ExpenseId != 0)
                {
                    var rec = await db.Expence.FindAsync(model.expense.ExpenseId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.ExpenceAmount = model.expense.Expense;
                    rec.CurrencyId = model.expense.ECurrencyId;
                    rec.ExpenceTypeId = model.expense.ExpenseTypeId;
                    rec.ExpenceDate = model.expense.ExpenseDate;
                    rec.Detail = model.expense.Detail;

                    db.Expence.Update(rec);
                   
                    var rec4 = db.CashInHand.Where(x => x.ExpenseId == rec.ExpenceId).FirstOrDefault();
                    rec4.Debit = model.expense.Expense;
                    rec4.Date = model.expense.ExpenseDate;
                    rec4.CurrencyId = model.expense.ECurrencyId;

                    db.CashInHand.Update(rec4);

                    await db.SaveChangesAsync();

                    return Json("ریکارډ تغیر شو.");
                }
                else
                {
                    Expence c = new Expence()
                    {
                        ExpenceAmount = model.expense.Expense,
                        CurrencyId = model.expense.ECurrencyId,
                        ExpenceTypeId = model.expense.ExpenseTypeId,
                        ExpenceDate = model.expense.ExpenseDate,
                        Detail = model.expense.Detail,
                        DepartmentId =depId
                    };
                 
                    db.Expence.Add(c);
                    await db.SaveChangesAsync();

                    CashInHand ch = new CashInHand()
                    {
                        Debit = model.expense.Expense,
                        Credit = 0,
                        Date = model.expense.ExpenseDate,
                        CurrencyId = model.expense.ECurrencyId,
                        ExpenseId = c.ExpenceId,
                        DepartmentId =depId
                    };
                    db.CashInHand.Add(ch);
                    await db.SaveChangesAsync();
                    return Json(" مصرف ریکارډ ذخیره شو.");
                }

            }
            return View("Error");
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteExpenseType(AllViewModel model)
        {
            if (model.expenseType.ExpenseTypeId != 0)
            {
                var comp = await db.ExpenceType.FindAsync(model.expenseType.ExpenseTypeId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    db.ExpenceType.Remove(comp);
                    await db.SaveChangesAsync();
                    return Json("ریکارډ حذف شو");
                }
                catch (Exception)
                {

                    return Json("تاسو نه شی کولی نوموړی ریکارد حذف کړی");
                }
            }
            return NotFound();

        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteExpense(int cId)
        {
            if (cId != 0)
            {
                var comp = await db.Expence.FindAsync(cId);
                if (comp == null)
                {
                    return View("Error");
                }
                db.Expence.Remove(comp);
                await db.SaveChangesAsync();
                return Json("deleted");
            }
            return NotFound();

        }

        public IActionResult BillExpenses()
        {
            return View();
        }

        public JsonResult FetchPurchaseExpense()
        {

            var result = (from i in db.PurExpense
                          select new
                          {
                              id = i.PurExpenseId,
                              billNo = i.BillNo,
                              dealer = db.Dealer.Where(x=>x.DealerId==i.DealerId).Select(x=>x.Name).FirstOrDefault(),
                              desc = i.Desc??"",
                              date = Convert.ToDateTime(i.Date).ToShortDateString(),
                              currency = db.Currency.Where(x => x.CurrencyId == i.CurrencyId).Select(x => x.Currency1).FirstOrDefault(),
                              amount = i.Amount,
                          }).OrderByDescending(x => x.id).ToList();

            return Json(result);

        }
    }
}