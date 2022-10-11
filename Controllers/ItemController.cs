using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;
using GeneralSalesDb.Models.ViewModels;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Admin Department,Purchase Department,Stock Department")]
    public class ItemController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        int depId = 0;

        public ItemController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
          
        }

        [HttpGet]
        public IActionResult Item()
        { 
           depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var Category = (from d in db.Category
                            where d.DepartmentId == depId
                            select new SelectListItem()
                            {
                                Text = d.Category1,
                                Value = d.CategoryId.ToString()
                            }).ToList();

            ViewBag.Category = Category;
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> ItemSave(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                if (model.item.ItemId != 0)
                {
                    var rec = await db.Item.FindAsync(model.item.ItemId);
                    if (rec == null)
                    {
                        return View("Error");
                    }


                    rec.ItemId = model.item.ItemId;
                    rec.Name = model.item.Name;
                    rec.CategoryId = model.item.CategoryId;

                    db.Item.Update(rec);
                    
                    await db.SaveChangesAsync();

                    return Json("ریکارد تغیر شو");

                }
                else
                {
                    try
                    {
                        Item it = new Item()
                        {
                            Name = model.item.Name,
                            CategoryId = model.item.CategoryId,
                            DepartmentId = depId
                        };
                        db.Item.Add(it);

                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {

                        throw new Exception(e.Message);
                    }

                    return Json("ریکارد ذخیره شو");
                }

            }
            return View("Error");

        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> SaveItemCategory(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                if (model.itemCategory.CategoryId != 0)
                {
                    var rec = await db.Category.FindAsync(model.itemCategory.CategoryId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.Category1 = model.itemCategory.Category;


                    db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.Entry(rec).Property(d => d.Category1).IsModified = true;

                    await db.SaveChangesAsync();

                    return Json("ریکارد تغیر شو");
                    //return View("Item");
                }
                else
                {
                    try
                    {
                        Category it = new Category()
                        {
                            Category1 = model.itemCategory.Category,
                            DepartmentId = depId

                        };
                        db.Category.Add(it);
                        await db.SaveChangesAsync();

                        return Json("ریکارد ذخیره شو");
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                }

            }
            return View("Error");

        }

        public JsonResult LoadItem()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var rec = (from i in db.Item where i.DepartmentId==depId
                       join c in db.Category on i.CategoryId equals c.CategoryId
                       select new
                       {
                           i.ItemId,
                           i.Name,
                           c.Category1,
                           c.CategoryId,
                       }).ToList().OrderByDescending(d => d.ItemId);
            return new JsonResult(rec);
        }

        public JsonResult LoadCategory()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var rec = db.Category.Where(x=>x.DepartmentId==depId).ToList().OrderByDescending(d => d.CategoryId);
            return Json(rec);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteCategory(AllViewModel model)
        {
            if (model.itemCategory.CategoryId != 0)
            {
                var comp = await db.Category.FindAsync(model.itemCategory.CategoryId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Category.Remove(comp);
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

        public IActionResult IsItemExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            if (model.item.ItemId != 0)
                return Json(true);
            var rec = db.Item.Any(d => d.Name == model.item.Name && d.DepartmentId == depId);

            if (!rec)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public IActionResult IsItemCategoryExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            if (model.itemCategory.CategoryId != 0)
                return Json(true);
            var rec = db.Category.Any(d => d.Category1 == model.itemCategory.Category && d.DepartmentId == depId);

            if (!rec)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [Authorize(Roles = "Admin Department,Purchase Department,Stock Department")]
        public JsonResult ShowItemQuantity()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var SelectedItemRecored = (from i in db.Item
                                       join pr in db.StockItems on i.ItemId equals pr.ItemId
                                       where pr.DepartmentId == depId
                                       select new
                                       {
                                           itemid = i.ItemId,
                                           itemname = i.Name,
                                           shortageQuantity = pr.ShortageQty,
                                           item2id = pr.StockItemId,
                                           purchaseprice = pr.Price,
                                           currencyid = pr.CurrencyId,
                                           quantity = pr.Quantity,
                                           inunit = pr.InUnit,
                                       }).Where(x => x.quantity <= x.shortageQuantity && x.quantity != 0).ToList();

            return Json(SelectedItemRecored);
        }

        [Authorize(Roles = "Admin Department,Purchase Department,Stock Department")]
        public IActionResult ItemDetails(int id)
        {
            ViewBag.Id = id;
            return View();
        }


        [Authorize(Roles = "Admin Department,Purchase Department,Stock Department")]
        public JsonResult showSpecificItemRecord(int id)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var SelectedItemRecored = (from i in db.Item
                                       join i2 in db.StockItems on i.ItemId equals i2.ItemId
                                       join curr in db.Currency on i2.CurrencyId equals curr.CurrencyId
                                       where i2.StockItemId == id && i2.DepartmentId == depId
                                       select new
                                       {
                                           itemid = i.ItemId,
                                           itemname = i.Name,
                                           currency = curr.Currency1,
                                           unit = i2.Unit.Unit1,
                                           amountinpackage = i2.InUnit,
                                           purchaseprice = i2.Price,
                                           quantity = i2.Quantity,
                                           currencyid = i2.CurrencyId,
                                           type = i2.Type == "Processed" ? "پراسس شوې" : "خام مواد",
                                       }).ToList();
            return Json(SelectedItemRecored);
        }

    }
}