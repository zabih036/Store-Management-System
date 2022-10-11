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

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Purchase Department,Admin Department,Stock Department")]
    public class ManageSmallTablesController : Controller
    {
        
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        int depId = 0;
        public ManageSmallTablesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Management()
        {

            return View();
        }

        public IActionResult Unit()
        {

            return View();
        }

        public JsonResult LoadUnit()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var rec = db.Unit.Where(x=>x.DepartmentId == depId).ToList().OrderByDescending(d => d.UnitId);
            return Json(rec);
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> SaveUnit(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                if (model.unit.UnitId != 0)
                {
                    var rec = await db.Unit.FindAsync(model.unit.UnitId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.Unit1 = model.unit.Unit;


                    db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.Entry(rec).Property(d => d.Unit1).IsModified = true;

                    await db.SaveChangesAsync();

                    return Json("واحد تغیر شو.");
                }
                else
                {
                    Unit c = new Unit()
                    {
                        Unit1 = model.unit.Unit,
                        DepartmentId =depId
                    };
                    db.Unit.Add(c);
                    await db.SaveChangesAsync();
                    return Json("واحد ذخیره شو.");
                }

            }
            return View("Error");
        }

        [Authorize(Policy = ("DeleteRolePolicy"))]
        public async Task<IActionResult> DeleteUnit(AllViewModel model)
        {
            if (model.unit.UnitId != 0)
            {
                var comp = await db.Unit.FindAsync(model.unit.UnitId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Unit.Remove(comp);
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

        [Authorize(Roles = "Admin Department")]
        public IActionResult Currency_Stock()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var role = _userManager.GetUsersInRoleAsync("Admin Department").Result;
            var user = role.FirstOrDefault();
            var email = user.Email;
            var employeeId = db.Employee.Where(d => d.Email == email).Select(r => r.EmployeeId).FirstOrDefault();
            var employees = (from d in db.Employee
                             where d.EmployeeId != employeeId && d.DepartmentId == depId
                             select new SelectListItem()
                             {
                                 Text = d.Name,
                                 Value = d.EmployeeId.ToString()
                             }).ToList();
            ViewBag.EmployeeId = employees;
            return View();
        }

        public JsonResult LoadStock()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var rec = (from d in db.Stock
                       join c in db.Employee on d.Responsible equals c.EmployeeId
                       where d.DepartmentId == depId
                       select new
                       {
                           stockId = d.StockId,
                           stock = d.Stock1,
                           employee = c.Name,
                           employeeId = c.EmployeeId,
                           location = d.Location,
                           details = d.Details,
                       }).ToList().OrderByDescending(d => d.stockId);
            return Json(rec);
        }

        public IActionResult IsUnitExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            if (model.unit.UnitId != 0)
            {
                return Json(true);
            }
            var rec = db.Unit.Any(d => d.Unit1 == model.unit.Unit && d.DepartmentId ==depId);

            if (!rec)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public IActionResult IsStockExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            if (model.stock.StockId != 0)
            {
                return Json(true);
            }
            var rec = db.Stock.Any(d => d.Stock1 == model.stock.Stock && d.DepartmentId == depId);

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
        public async Task<IActionResult> SaveStock(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                if (model.stock.StockId != 0)
                {
                    var rec = await db.Stock.FindAsync(model.stock.StockId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.Stock1 = model.stock.Stock;
                    rec.Responsible = model.stock.EmployeeId;
                    rec.Location = model.stock.Location;
                    rec.Details = model.stock.Details;


                    db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.Entry(rec).Property(d => d.Stock1).IsModified = true;

                    await db.SaveChangesAsync();

                    return Json("د ګودام نوم تغیر شو.");
                }
                else
                {
                    Stock c = new Stock()
                    {
                        Stock1 = model.stock.Stock,
                        Responsible = model.stock.EmployeeId,
                        Location = model.stock.Location,
                        Details = model.stock.Details,
                        DepartmentId = depId

                    };
                    db.Stock.Add(c);
                    await db.SaveChangesAsync();
                    return Json("نوی ګودام ذخیره شو.");
                }

            }
            return View("Error");
        }

        [Authorize(Policy = ("DeleteRolePolicy"))]
        public async Task<IActionResult> DeleteStock(AllViewModel model)
        {
            if (model.stock.StockId != 0)
            {
                var comp = await db.Stock.FindAsync(model.stock.StockId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Stock.Remove(comp);
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
    }
}