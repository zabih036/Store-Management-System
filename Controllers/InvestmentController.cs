using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.ViewModels;
using GeneralSalesDb.Models.Model;
using Microsoft.EntityFrameworkCore;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Admin Department,Finance Department")]
    public class InvestmentController : Controller
    {

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        GeneralSalesDbContext db = new GeneralSalesDbContext();

        int depId = 0;
        public InvestmentController(UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Investor()
        {
            if (TempData["updated"] != null)
            {
                ViewBag.Alert = TempData["updated"];
            }
            else
            {
                ViewBag.Alert = "empty";
            }
            return View();
        }

        public JsonResult LoadInvestor()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = db.Investor.Where(x => x.DepartmentId == depId).ToList().OrderByDescending(d => d.InvestorId);
            return Json(rec);
        }

        [HttpPost]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> InvestorRegistration(AllViewModel model, [FromForm] IFormFile img)
        {
            if (ModelState.IsValid)
            {

                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                string uploadPath = "";
                string upload = "";
                if (img == null)
                {
                    if (model.investor.InvestorId == 0)
                        upload = "/images/StaticImages/browse.png";
                }
                else
                {


                    var fileName = model.investor.Email + Guid.NewGuid().ToString().Replace("_", "") + Path.GetExtension(img.FileName);
                    upload = Path.Combine("Images/InvestorImage/", fileName);
                    uploadPath = Path.Combine(hostingEnvironment.WebRootPath, upload);
                    img.CopyTo(new FileStream(uploadPath, FileMode.Create));
                    upload = "/" + upload;

                }


                if (model.investor.InvestorId != 0)
                {

                    var rec = await db.Investor.FindAsync(model.investor.InvestorId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.Name = model.investor.Name;
                    rec.Phone = model.investor.Phone.ToString();
                    rec.Email = model.investor.Email;
                    rec.RegistrationDate = model.investor.RegistrationDate;
                    rec.Description = model.investor.Description;

                    db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    if (img != null && model.investor.defalut == "0")
                    {
                        db.Entry(rec).Property(d => d.Image).IsModified = true;

                    }
                    else if (model.investor.defalut != "0")
                    {
                        db.Entry(rec).Property(d => d.Image).IsModified = true;

                    }

                    await db.SaveChangesAsync();
                    TempData["updated"] = " ریکارډ تغیر شو";
                    return RedirectToAction("Investor", "Investment");

                }
                else
                {
                    try
                    {
                        Investor it = new Investor
                        {
                            Name = model.investor.Name,
                            Phone = model.investor.Phone.ToString(),
                            Email = model.investor.Email,
                            RegistrationDate = Convert.ToDateTime(model.investor.RegistrationDate.ToShortDateString()).Date,
                            Description = model.investor.Description,
                            Image = upload,
                            DepartmentId = depId
                        };
                        db.Investor.Add(it);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {

                        throw new Exception(e.Message);
                    }
                    TempData["updated"] = " ریکارډ ذخیره شو";
                    return RedirectToAction("Investor", "Investment");
                }

            }
            return View("Error");

        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> Deleteinvestor(AllViewModel model)
        {
            if (model.investor.InvestorId != 0)
            {

                var comp = await db.Investor.FindAsync(model.investor.InvestorId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Investor.Remove(comp);
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

        public IActionResult LoadinvestorAllPayments(AllViewModel model)
        {
            if (model.investor.InvestorId != 0)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var comp = (from s in db.BenifitPayment
                            where s.InvestorId == model.investor.InvestorId && s.DepartmentId == depId //model.investor.investorId
                            select new
                            {
                                paidDate = Convert.ToDateTime(s.PaidDate).Date.ToShortDateString(),
                                paidAmount = s.Amount
                            }).ToList();
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {

                    return Json(comp);
                }
                catch (Exception)
                {

                    return Json("error");
                }
            }
            return NotFound();

        }

        [Authorize(Policy = "InsertRolePolicy")]
        public IActionResult AddInvestment()
        {
            return View();
        }

        public JsonResult GetAllDeals(int DealerID)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var DealV = (from i in db.InvestMoney
                         where i.DepartmentId == depId
                         join iv in db.Investor on i.InvestorId equals iv.InvestorId
                         join cu in db.Currency on i.CurrencyId equals cu.CurrencyId
                         select new
                         {
                             dealid = i.InvestMoneyId,
                             debit = i.Debit,
                             credit = i.Credit,
                             type= i.Type,
                             currencyid = cu.CurrencyId,
                             currency = cu.Currency1,
                             dealerid = iv.InvestorId,
                             phone = iv.Phone,
                             dealer = iv.Name,
                             employee = db.Employee.Where(x => x.Email == i.Employee).Select(x => x.Name).FirstOrDefault(),
                             date = Convert.ToDateTime(i.Date).ToShortDateString(),
                             detail = i.Description??""
                         }).Where(x => x.dealerid == DealerID).OrderByDescending(r => r.dealid);

            return Json(DealV);
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> NewDeal(InvestDealViewModel deal)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var email = _userManager.GetUserAsync(User).Result.Email;

            if (ModelState.IsValid)
            {

                if (deal.DealId == 0)
                {
                    InvestMoney de = new InvestMoney
                    {
                        Credit = deal.Credit,
                        Debit = deal.Debit,
                        InvestorId = deal.DealerID,
                        CurrencyId = deal.CurrencyID,
                        Employee = email,
                        Date = Convert.ToDateTime(DateTime.Now.ToShortDateString()),
                        Description = deal.Detail,
                        Type = deal.Type,
                        DepartmentId = depId
                    };

                    db.InvestMoney.Add(de);
                    await db.SaveChangesAsync();

                    if (deal.Type != "وسایل")
                    {
                        CashInHand ch = new CashInHand()
                        {
                            CurrencyId = deal.CurrencyID,
                            Credit = deal.Credit,
                            Debit = deal.Debit,
                            Date = DateTime.Now.Date,
                            Description = " پانګونه",
                            InvestId = de.InvestMoneyId,
                            DepartmentId = depId,
                        };
                        db.CashInHand.Add(ch);
                        await db.SaveChangesAsync();
                    }

                    return Json("ریکارډ ذخیره شو");
                }
                else
                {
                    var deals = db.InvestMoney.Where(x=>x.InvestMoneyId == deal.DealId).FirstOrDefault();

                    if (deals != null)
                    {
                        deals.Credit = deal.Credit;
                        deals.Debit = deal.Debit;
                        deals.Type = deal.Type;
                        deals.Description = deal.Detail;

                        db.InvestMoney.Update(deals);

                        var cash = db.CashInHand.Where(x => x.InvestId == deals.InvestMoneyId).FirstOrDefault();


                        if (cash != null)
                        {
                            if (deal.Type == "وسایل")
                            {
                                db.CashInHand.Remove(cash);
                            }
                            else
                            {
                                cash.Debit = deal.Debit;
                                cash.Credit = deal.Credit;
                                cash.Description = deal.Detail;

                                db.CashInHand.Update(cash);
                            }


                        }

                        if (deal.Type == "نغدي پیسې" &&cash==null)
                        {
                            CashInHand csh = new CashInHand
                            {
                                Debit = deal.Debit,
                                Credit = deal.Credit,
                                CurrencyId = deals.CurrencyId,
                                Date = DateTime.Now.Date,
                                Description = deal.Detail,
                                InvestId = deals.InvestMoneyId,
                                DepartmentId = depId,

                            };

                            db.CashInHand.Add(csh);

                        }

                        await db.SaveChangesAsync();

                        return Json("ریکارد تغیر شو");
                    }
                }
            }
           
            return Json("اشتباه");
        }

        public JsonResult FetchDealers()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var SelectedItemRecored = (from i in db.Investor
                                       where i.DepartmentId == depId
                                       select new
                                       {
                                           dealerid = i.InvestorId,
                                           name = i.Name,
                                           phone = i.Phone,
                                           email = i.Email,
                                       }).ToList();
            return Json(SelectedItemRecored);
        }

        public IActionResult LastYearAssets()
        {
            return View();
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<JsonResult> AddAsset(LastAssetsViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var email = _userManager.GetUserAsync(User).Result.Email;

                if (model.AssetId == 0)
                {
                    var rec = db.Assets.Where(x => x.Dates == model.Date).FirstOrDefault();

                    if (rec != null)
                    {
                        return Json("نوموړی کال سرمایه ثبت شوې");
                    }

                    Assets asset = new Assets()
                    {
                        AfAsset = model.AfAsset,
                        PkAsset = model.PkAsset,
                        UsAsset = model.UsAsset,
                        Dates = model.Date,
                        Desc = model.Desc,
                        DepartmentId = depId
                    };
                    db.Assets.Add(asset);
                    await db.SaveChangesAsync();
                    return Json("ریکارډ ذخیره شو!!!");
                }
                else
                {
                    var rec = await db.Assets.FindAsync(model.AssetId);

                    rec.AfAsset = model.AfAsset;
                    rec.PkAsset = model.PkAsset;
                    rec.UsAsset = model.UsAsset;
                    rec.Desc = model.Desc;

                    db.Assets.Update(rec);
                    await db.SaveChangesAsync();
                    return Json("ریکارډ تغیر شو!!!");
                }
            }

            return Json("اشتباه");
        }

        public JsonResult FetchAssets()
        {
            var email = _userManager.GetUserAsync(User).Result.Email;
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var AcitveNotes = (from N in db.Assets
                               where  N.DepartmentId == depId
                               select new
                               {
                                   id = N.AssetId,
                                   afasset = N.AfAsset,
                                   pkasset = N.PkAsset,
                                   usasset = N.UsAsset,
                                   date = N.Dates,
                                   desc = N.Desc??""
                               }).OrderByDescending(x=>x.id).ToList();

            return Json(AcitveNotes);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<JsonResult> DeleteAsset(int assetId)
        {
            var record =await db.Assets.FindAsync(assetId);
            if (record != null)
            {

            db.Assets.Remove(record);
            await db.SaveChangesAsync();
            return Json("نوټ حذف شو !!!");
            }
            return Json("اشتباه !!!");
        }

        public IActionResult Cash()
        {
            return View();
        }

        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<JsonResult> AddCash(CashViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var email = _userManager.GetUserAsync(User).Result.Email;

                if (model.CashId == 0)
                {
                    var cr = 0.0;
                    var deb = 0.0;

                    if (model.Type == "Credit")
                    {
                        cr = model.Amount;
                    }
                    else
                    {
                        deb = model.Amount;
                    }

                    CashTable csh = new CashTable()
                    {
                        Credit = cr,
                        Debit = deb,
                        CurrencyId = model.CurrencyId,
                        Date = DateTime.Now.Date,
                        Desc = model.Desc,
                        Employee = email,
                        DepartmentId = depId
                    };
                    db.CashTable.Add(csh);

                    await db.SaveChangesAsync();

                    CashInHand cash = new CashInHand()
                    {
                        Credit = cr,
                        Debit = deb,
                        CurrencyId = model.CurrencyId,
                        Date = DateTime.Now.Date,
                        Description = model.Desc,
                        CashRecordId = csh.CashRecordId,
                        DepartmentId = depId,
                    };
                    db.CashInHand.Add(cash);
                    await db.SaveChangesAsync();
                    return Json("ریکارډ ذخیره شو!!!");
                }
                else
                {
                    var cr = 0.0;
                    var deb = 0.0;

                    if (model.Type == "Credit")
                    {
                        cr = model.Amount;
                    }
                    else
                    {
                        deb = model.Amount;
                    }

                    var rec = await db.CashTable.FindAsync(model.CashId);
                    var rec2 = await db.CashInHand.Where(x=>x.CashRecordId== model.CashId).FirstOrDefaultAsync();

                    rec.Debit = deb;
                    rec.Credit = cr;
                    rec.CurrencyId = model.CurrencyId;
                    rec.Desc = model.Desc;
                    rec.Employee = email;

                    rec2.Debit = deb;
                    rec2.Credit = cr;
                    rec2.CurrencyId = model.CurrencyId;
                    rec2.Description = model.Desc;

                    db.CashTable.Update(rec);

                    db.CashInHand.Update(rec2);

                    await db.SaveChangesAsync();
                    return Json("ریکارډ تغیر شو!!!");
                }
            }

            return Json("اشتباه");
        }

        public JsonResult FetchCash()
        {
            var email = _userManager.GetUserAsync(User).Result.Email;
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var AcitveNotes = (from N in db.CashTable
                               where N.DepartmentId == depId
                               select new
                               {
                                   id = N.CashRecordId,
                                   debit = N.Debit,
                                   credit = N.Credit,
                                   currency = N.Currency.Currency1,
                                   currencyId = N.CurrencyId,
                                   date =Convert.ToDateTime( N.Date).ToShortDateString(),
                                   employee = db.Employee.Where(x=>x.Email == N.Employee).Select(x=>x.Name).FirstOrDefault(),
                                   desc = N.Desc ?? ""
                               }).OrderByDescending(x => x.id).ToList();

            return Json(AcitveNotes);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<JsonResult> DeleteCash(int assetId)
        {
            var record = await db.Assets.FindAsync(assetId);
            if (record != null)
            {

                db.Assets.Remove(record);
                await db.SaveChangesAsync();
                return Json("نوټ حذف شو !!!");
            }
            return Json("اشتباه !!!");
        }

    }
}
