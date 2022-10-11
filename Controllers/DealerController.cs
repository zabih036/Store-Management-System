using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;
using GeneralSalesDb.Models.ViewModels;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Finance Department,Admin Department,Purchase Department,Stock Department")]
    public class DealerController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> userManager;
        int depId = 0;
        public DealerController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// Dealers ///
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> DealerRegistration(DealerViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.DealerId != 0)
                {
                    var rec = await db.Dealer.FindAsync(model.DealerId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.DealerId = model.DealerId;
                    rec.Name = model.Name;
                    rec.Fname = model.FName;
                    rec.Address = model.Address;
                    rec.Phone = model.Phone.ToString();

                    db.Dealer.Update(rec);
                    await db.SaveChangesAsync();

                    return Json("کهاتدار معلومات تغیر شو");
                }
                else
                {
                    try
                    {
                        Dealer dealer = new Dealer()
                        {
                            Name = model.Name,
                            Fname = model.FName,
                            Address = model.Address,
                            Phone = model.Phone.ToString(),
                            Status = "Unlocked",
                            DepartmentId = depId,
                        };
                        db.Dealer.Add(dealer);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    return Json("کهاتدار معلومات ذخیره شو");
                }
            }
            return View("Error");
        }

        public JsonResult FetchDealers()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealer = (from d in db.Dealer
                          where d.DepartmentId == depId
                                       select new
                                       {
                                           dealerId = d.DealerId,
                                           name = d.Name,
                                           fName = d.Fname,
                                           address = d.Address,
                                           phone = d.Phone,
                                           status = d.Status,
                                       }).ToList().OrderByDescending(r => r.dealerId);
            return Json(dealer);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteDealer(DealerViewModel model)
        {
            if (model.DealerId != 0)
            {
                var dealer = await db.Dealer.FindAsync(model.DealerId);
                if (dealer == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Dealer.Remove(dealer);
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

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> LockDealerAccount(int dealerId, string trueFalse)
        {
            var user = await db.Dealer.FindAsync(dealerId);

            if (trueFalse == "lock")
            {
                user.Status = "Locked";

                db.Dealer.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }
            else
            {

                user.Status = "Unlocked";

                db.Dealer.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }


        }

        /// Customers ///
       [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> CustomerRegistration(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.CustomerId != 0)
                {
                    var rec = await db.Customer.FindAsync(model.CustomerId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.CustomerId = model.CustomerId;
                    rec.Name = model.CusName;
                    rec.Fname = model.CusFName;
                    rec.Address = model.CusAddress;
                    rec.Phone = model.CusPhone.ToString();

                    db.Customer.Update(rec);
                    await db.SaveChangesAsync();

                    return Json("مشترې معلومات تغیر شو");
                }
                else
                {
                    try
                    {
                        Customer Customer = new Customer()
                        {
                            Name = model.CusName,
                            Fname = model.CusFName,
                            Address = model.CusAddress,
                            Phone = model.CusPhone.ToString(),
                            Status = "Unlocked",
                            DepartmentId = depId,
                        };
                        db.Customer.Add(Customer);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    return Json("مشترې معلومات ذخیره شو");
                }
            }
            return View("Error");
        }

        public JsonResult FetchCustomers()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var Customer = (from d in db.Customer
                            where d.DepartmentId==depId
                          select new
                          {
                              customerId = d.CustomerId,
                              name = d.Name,
                              fName = d.Fname,
                              address = d.Address,
                              phone = d.Phone,
                              status = d.Status,
                          }).ToList().OrderByDescending(r => r.customerId);
            return Json(Customer);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteCustomer(CustomerViewModel model)
        {
            if (model.CustomerId != 0)
            {
                var Customer = await db.Customer.FindAsync(model.CustomerId);
                if (Customer == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Customer.Remove(Customer);
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

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> LockAccount(int dealerId,string trueFalse)
        {
            var user = await db.Customer.FindAsync(dealerId);

            if (trueFalse == "lock")
            {
                user.Status = "Locked";

                db.Customer.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }
            else
            {

                user.Status = "Unlocked";

                db.Customer.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }


        }


        /// Banks ///
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> BankRegistration(BankViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.BankId != 0)
                {
                    var rec = await db.Bank.FindAsync(model.BankId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.BankId = model.BankId;
                    rec.Name = model.NameB;
                    rec.Address = model.AddressB;
                    rec.Phone = model.PhoneB.ToString();

                    db.Bank.Update(rec);
                    await db.SaveChangesAsync();

                    return Json($"{model.NameB} معلومات تغیر شو");
                }
                else
                {
                    try
                    {
                        Bank bank = new Bank()
                        {
                            Name = model.NameB,
                            Address = model.AddressB,
                            Phone = model.PhoneB.ToString(),
                            Status = "Unlocked",
                            DepartmentId = depId,
                        };
                        db.Bank.Add(bank);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    return Json($"{model.NameB} معلومات ذخیره شو");
                }
            }
            return View("Error");
        }

        public JsonResult FetchBanks()
        {
            depId = userManager.GetUserAsync(User).Result.DepartmentId;

            var dealer = (from d in db.Bank
                          where d.DepartmentId == depId
                          select new
                          {
                              bankId = d.BankId,
                              name = d.Name,
                              address = d.Address,
                              phone = d.Phone,
                              status = d.Status,
                          }).ToList().OrderByDescending(r => r.bankId);
            return Json(dealer);
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteBank(int bankId)
        {
            if (bankId != 0)
            {
                var bank = await db.Bank.FindAsync(bankId);
                if (bank == null)
                {
                    return View("Error");
                }

                try
                {
                    db.Bank.Remove(bank);
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

        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> LockBankAccount(int bankId, string trueFalse)
        {
            var user = await db.Bank.FindAsync(bankId);

            if (trueFalse == "lock")
            {
                user.Status = "Locked";

                db.Bank.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }
            else
            {

                user.Status = "Unlocked";

                db.Bank.Update(user);

                await db.SaveChangesAsync();

                return Json("true");
            }
        }
    }
}
