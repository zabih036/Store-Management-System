using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.AccountViewModels;
using GeneralSalesDb.Models.ViewModels;
using GeneralSalesDb.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using GeneralSalesDb.Models.Model;
using Microsoft.EntityFrameworkCore;

namespace GeneralSalesDb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        readonly GeneralSalesDbContext db = new GeneralSalesDbContext();

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        public IConfiguration configuration;

        int depId = 0;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _logger = logger;
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        [TempData]
        public string ErrorMessage { get; set; }
        [Authorize(Roles = "Admin Department")]
        [HttpPost]
        public async Task<IActionResult> AddClaimToUser(AllViewModel model, [FromForm] string radio)
        {
            var user = await _userManager.FindByEmailAsync(model.claim.Email);

            if (user == null)
            {
                return NotFound();
            }

            var ClaimsOfUser = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, ClaimsOfUser);

            if (!result.Succeeded)
            {
                return View("Error");
            }

            if (radio == "None")
            {
                return Json("نوموړی کارمند کوم صلاحیت نه لرې");
            }
            if (radio == "View")
            {
                result = await _userManager.AddClaimAsync(user, new Claim("View Role", "View Role"));

            }
            if (radio == "Insert")
            {
                result = await _userManager.AddClaimsAsync(user, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role")
                });
            }
            if (radio == "Edit")
            {
                result = await _userManager.AddClaimsAsync(user, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role"),
                    new Claim("Edit Role", "Edit Role")
                });
            }
            if (radio == "Delete")
            {
                result = await _userManager.AddClaimsAsync(user, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role"),
                    new Claim("Edit Role", "Edit Role"),
                     new Claim("Delete Role", "Delete Role")
                });
            }
            if (!result.Succeeded)
            {
                return View("Error");
            }

            return Json("کارمند ته انتخاب شوی صلاحیت ورکړل شو.");
        }

        [Authorize(Roles = "Admin Department")]
        public IActionResult LoadAccounts()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = _userManager.Users.Where(x => x.DepartmentId == depId && x.Email !="developer@gmail.com" && x.Email != "superadmin@gmail.com").ToList();
            if (rec == null)
            {
                return NotFound();
            }
            return Json(rec);
        }

        [Authorize(Roles = "Admin Department")]
        public async Task<IActionResult> ChangeUserRole(AllViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.register.email2);
                if (user == null)
                {
                    return View("Error");
                }
                var role = await _userManager.RemoveFromRoleAsync(user, model.register.role);
                if (!role.Succeeded)
                {
                    return View("Error");
                }

                var assign = await _userManager.AddToRoleAsync(user, model.register.id2);


                return Json("د کارمند رول تغیر شو");

            }
            catch (Exception)
            {

                return Json("اشتباه");
            }

        }

        [Authorize(Roles = "Admin Department")]
        public async Task<IActionResult> LoadUserClaims(AllViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.claim.Email);

            var logs = (from l in db.UserLoginDetail
                        where l.UserId == model.claim.Email
                        select new
                        {
                            email = l.UserId,
                            loginDate = Convert.ToDateTime(l.LoginDate).ToString("yyyy/MM/dd hh:mm:ss"), /*Convert.ToDateTime(l.LoginDate).Date.ToUniversalTime()*/ /*+"-" + Convert.ToDateTime(l.LoginDate).ToLocalTime().ToString("HH:mm:ss")*/
                            logoutDate = Convert.ToDateTime(l.LogoutDate).ToString("yyyy/MM/dd hh:mm:ss"),
                        }).ToList();
            var role = await _userManager.GetRolesAsync(user);


            if (user == null)
                return NotFound();
            var Claims = await _userManager.GetClaimsAsync(user);
            if (Claims == null)
            {
                return Json("NoRoles");
            }
            var rec = new
            {
                claims = Claims.Count(),
                role = role,
                logs = logs
            };
            return Json(rec);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    UserLoginDetail log = new UserLoginDetail()
                    {
                        UserId = model.Email,
                        LoginDate = DateTime.Now.ToLocalTime(),
                    };
                    db.UserLoginDetail.Add(log);

                    await db.SaveChangesAsync();

                    return RedirectToLocal(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, " ستاسو اکاونټ د 15 دقیقو لپاره بلاک شو .");
                    return View(model);
                }
                if (result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, " ستاسو اکاونټ د ریس لخوا بلاک شوی .");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "بریښنالیک او یا پټ نوم مو غلط دی");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin Department")]
        public async Task<IActionResult> LockAccount(AllViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.register.email);
            if (model.register.trueOrfalse == "lock")
            {
                user.EmailConfirmed = false;
                await _userManager.UpdateAsync(user);
                return Json("true");
            }
            else
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return Json("true");
            }


        }

        [Authorize(Roles = "Admin Department")]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteAccount(AllViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.register.email);

            var r = await _userManager.GetRolesAsync(user);

            if (r[0].ToString() == "Admin Department")
            {
                return Json("تاسو نه شی کولی نوموړی اکاونټ حذف کړئ");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json("اکاونت حذف شو");
            }
            else { return Json("اکاونت حذف نه شو"); }
        }

        [Authorize(Roles = "Admin Department")]
        public IActionResult Register(string returnUrl = null)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var Employee = (from d in db.Employee
                            where d.DepartmentId == depId
                            select new SelectListItem()
                            {
                                Text = d.Name,
                                Value = d.EmployeeId.ToString()
                            }).ToList();
            ViewBag.Employee = Employee;

            var Roles = (from d in _roleManager.Roles
                         where d.Name!="Developer"&&d.Name!="Super Admin"
                         select new SelectListItem()
                         {
                             Text = d.Name,
                             Value = d.Name
                         }).ToList();
            ViewBag.Roles = Roles;
            ViewBag.Roles2 = Roles;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult IsEmployeeAccountExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = db.Employee.Where(d => d.EmployeeId == model.register.EmployeeId && d.DepartmentId == depId).FirstOrDefault();
            var Email = "";
            if (rec != null)
                Email = rec.Email;
            var user = _userManager.FindByEmailAsync(Email);

            if (user.Result == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin Department")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AllViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var rec = db.Employee.Where(e => e.EmployeeId == model.register.EmployeeId).FirstOrDefault();
                var user = new ApplicationUser { UserName = rec.Email, Email = rec.Email, EmployeeName = rec.Name, ImagePath = rec.Image, DepartmentId = depId };
                var result = await _userManager.CreateAsync(user, model.register.Password);
                if (result.Succeeded)
                {
                    var userToRole = await _userManager.FindByEmailAsync(rec.Email);
                    await _userManager.AddToRoleAsync(userToRole, model.register.id);


                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    //await _emailSender.SendEmailConfirmationAsync(rec.Email, callbackUrl);

                    //  await _signInManager.SignInAsync(user, isPersistent: false);
                    // _logger.LogInformation("User created a new account with password.");
                    return Json(rec.Name + " لپاره اکاونټ جوړ شو ");
                }
                //AddErrors(result);
                return Json("پټ نوم باید شپږ حروفه یو عدد او یو سیمبول ولرې");
            }

            // If we got this far, something failed, redisplay form
            return View("Error");
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _userManager.GetUserAsync(User).Result.Email;

                var rec1 = db.UserLoginDetail.Where(d => d.UserId == userId ).ToList().OrderByDescending(r => r.DetailId);
                UserLoginDetail rec;
                if (rec1 != null)
                {
                    rec = rec1.First();
                    if (rec != null)
                    {
                        rec.LogoutDate = DateTime.Now.ToLocalTime();

                        db.Entry(rec).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.Entry(rec).Property(r => r.LogoutDate).IsModified = true;
                        await db.SaveChangesAsync();
                    }
                    await _signInManager.SignOutAsync();
                }

                return RedirectToAction("Login", "Account");

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin Department")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(AllViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.forgot.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Json(1);

                }
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return Json(2);

                }

                var code = SendCodeToUser(user.EmployeeName, user.Email);
                if (code == 500)
                {
                    return Json(3);
                }
                else
                {

                    return Json(code);
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                //   $"مهربانی له مخې په دې بټنې کلیک وکړی تر څو مو پټ نوم تغیر کړی.: <a href='{callbackUrl}'>link</a>");

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddNewPassword(AllViewModel model)
        {

            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByEmailAsync(model.reset.userEmail);
                await _userManager.RemovePasswordAsync(user);
                var pass = await _userManager.AddPasswordAsync(user, model.reset.NewPassword);
                if (!pass.Succeeded)
                {
                    return Json("پټ نوم باید شپږ حروفه یو غټ حروف یو عدد او یو سیمبول ولرې");
                }
                await _signInManager.SignInAsync(user, isPersistent: false);
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                UserLoginDetail log = new UserLoginDetail()
                {
                    UserId = model.reset.userEmail,
                    LoginDate = DateTime.Now.ToLocalTime(),
                };
                db.UserLoginDetail.Add(log);
                await db.SaveChangesAsync();

                return Json("done");


            }

            return RedirectToAction(nameof(ForgotPassword));
        }

        public int SendCodeToUser(string fName, string userEmailOrPhone)
        {
            var systemEmail = configuration["SystemEmail"];
            var key = configuration["key"];
            Random ran = new Random();
            var code = ran.Next(1, 999999);
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(userEmailOrPhone);
                mail.From = new MailAddress("ziasaeedi2022@gmail.com");
                mail.Subject = "Password Reset";
                mail.Body = string.Format("Dear " + fName + "  <br/>  <br/>" + code + "<br/> is your password reset code");
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(systemEmail, key);
                smtp.EnableSsl = true;
                smtp.Send(mail);
                return code;
            }
            catch (Exception)
            {

                return 500;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return View("Error");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult SaveDocuments()
        {
            if (TempData["updated"] != null)
            {
                ViewBag.Alert = TempData["updated"];
            }
            else
            {
                ViewBag.Alert = "empty";
            }

            if (TempData["delete"] != null)
            {
                ViewBag.delete = TempData["delete"];
            }
            else
            {
                ViewBag.delete = "empty";
            }
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            ViewBag.Documents = db.Document.Where(x=>x.DepartmentId==depId).ToList();
            return View();
        }

        [Authorize(Roles = "Admin Department")]
        [HttpPost]
        public async Task<IActionResult> SaveDocumentss(DocumentViewModel model, [FromForm] IFormFile img)
        {
            var upload = "";
            var uploadPath = "";
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            if (img != null)
            {
                var fileName = Guid.NewGuid().ToString().Replace("_", "") + Path.GetExtension(img.FileName);
                upload = Path.Combine("Images/DocumentImage/", fileName);
                uploadPath = Path.Combine(hostingEnvironment.WebRootPath, upload);
                img.CopyTo(new FileStream(uploadPath, FileMode.Create));
                upload = "/" + upload;
                Document doc = new Document()
                {
                    DocumentDetails = model.DocumentDetails,
                    DocumentPicture = upload,
                    DepartmentId=depId
                };
                db.Document.Add(doc);
                await db.SaveChangesAsync();
                TempData["updated"] = "اسناد ذخیره شول";
                return RedirectToAction("SaveDocuments", "Account");
            }
            else { return View("Error"); }
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var rec = db.Document.Find(id);
            if (rec == null)
            {
                return View("Error");
            }
            db.Document.Remove(rec);
            await db.SaveChangesAsync();
            TempData["delete"] = "ریکارډ حذف شو";
            return RedirectToAction("SaveDocuments", "Account");
        }

        [Authorize(Policy = "DeleteRolePolicy")]
        [Authorize(Roles = "Admin Department")]
        public IActionResult Clear()
        {
            return View();
        }
        [Authorize(Policy = "DeleteRolePolicy")]
        [Authorize(Roles = "Admin Department")]
        public async Task<IActionResult> ClearAllTables()
        {
            db.CashTable.RemoveRange(await db.CashTable.ToListAsync());
            db.CashInHand.RemoveRange(await db.CashInHand.ToListAsync());
            db.CustomerDeal.RemoveRange(await db.CustomerDeal.ToListAsync());
            db.DealerDeal.RemoveRange(await db.DealerDeal.ToListAsync());
            db.Sale.RemoveRange(await db.Sale.ToListAsync());
            db.PrintTable.RemoveRange(await db.PrintTable.ToListAsync());
            db.OverTime.RemoveRange(await db.OverTime.ToListAsync());
            db.Note.RemoveRange(await db.Note.ToListAsync());
            db.Purchase.RemoveRange(await db.Purchase.ToListAsync());
            db.BankDeal.RemoveRange(await db.BankDeal.ToListAsync());
            db.EmployeeAttendance.RemoveRange(await db.EmployeeAttendance.ToListAsync());
            db.AttendanceSheet.RemoveRange(await db.AttendanceSheet.ToListAsync());
            db.Document.RemoveRange(await db.Document.ToListAsync());
            db.Expence.RemoveRange(await db.Expence.ToListAsync());
            db.ExpenceType.RemoveRange(await db.ExpenceType.ToListAsync());
            db.InvestMoney.RemoveRange(await db.InvestMoney.ToListAsync());
            db.Investor.RemoveRange(await db.Investor.ToListAsync());
            db.SalaryPayment.RemoveRange(await db.SalaryPayment.ToListAsync());
            db.StockItems.RemoveRange(await db.StockItems.ToListAsync());
            db.Stock.RemoveRange(await db.Stock.ToListAsync());
            db.Unit.RemoveRange(await db.Unit.ToListAsync());
            db.Item.RemoveRange(await db.Item.ToListAsync());
            db.Category.RemoveRange(await db.Category.ToListAsync());
            db.UserLoginDetail.RemoveRange(await db.UserLoginDetail.ToListAsync());
            db.Assets.RemoveRange(await db.Assets.ToListAsync());
            db.Customer.RemoveRange(await db.Customer.ToListAsync());
            db.Dealer.RemoveRange(await db.Dealer.ToListAsync());
            db.Bank.RemoveRange(await db.Bank.ToListAsync());
            db.Employee.RemoveRange(await db.Employee.Where(x=>x.Email!="admin@gmail.com").ToListAsync());
            db.SerialTable.RemoveRange(await db.SerialTable.ToListAsync());
            db.PurExpense.RemoveRange(await db.PurExpense.ToListAsync());

            db.AspNetUsers.RemoveRange(await db.AspNetUsers.Where(x => x.Email != "admin@gmail.com").ToListAsync());

            SerialTable serial = new SerialTable
            {
                SerialNumberId = 1,
                CashSerial = 1,
                BankSerial = 1,
                SaleSerial = 1,
                PurchaseSerial = 1,
                DepartmentId = 1,

            };

            db.SerialTable.Add(serial);

            await db.SaveChangesAsync();

            return Json("ډاټابیس ډاټا ختمه شوه");
        }

        [AllowAnonymous]
        public async Task<IActionResult> UpdateDatabase()
        {
            IdentityRole Admin = new IdentityRole()
            {
                Name = "Admin Department"
            };
            IdentityRole finance = new IdentityRole()
            {
                Name = "Finance Department"
            };

            IdentityRole sale = new IdentityRole()
            {
                Name = "Purchase Department"
            };
            IdentityRole purchase = new IdentityRole()
            {
                Name = "Sale Department"
            };
            IdentityRole stock = new IdentityRole()
            {
                Name= "Stock Department"
            };
            IdentityRole SD = new IdentityRole()
            {
                Name = "Super Admin"
            };

            IdentityRole DD = new IdentityRole()
            {
                Name = "Developer"
            };

            await _roleManager.CreateAsync(sale);
            await _roleManager.CreateAsync(stock);
            await _roleManager.CreateAsync(Admin);
            await _roleManager.CreateAsync(finance);
            await _roleManager.CreateAsync(purchase);
            await _roleManager.CreateAsync(SD);
            await _roleManager.CreateAsync(DD);


            var findadmin = _userManager.FindByEmailAsync("Admin@gmail.com");
            if (findadmin.Result==null)
            {
                Employee rec = new Employee()
                {
                    Email = "Admin@gmail.com",
                    Image = "/images/StaticImages/Admin.png",
                    Name = "AdminAccount",
                    HireDate = DateTime.Now.Date,
                    Salary = 0,
                    DepartmentId=1,
                };
                db.Employee.Add(rec);
                await db.SaveChangesAsync();

                var user = new ApplicationUser { UserName = "Admin@gmail.com", Email = "Admin@gmail.com", ImagePath = "/images/StaticImages/Admin.png", EmployeeName = "admin",DepartmentId=1 };
                var result = await _userManager.CreateAsync(user, "Khan1@");


                var us = await _userManager.FindByEmailAsync("Admin@gmail.com");

                us.EmailConfirmed = true;
                await _userManager.UpdateAsync(us);

                var res = await _userManager.AddToRoleAsync(us, "Admin Department");

                await _userManager.AddClaimsAsync(us, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role"),
                    new Claim("Edit Role", "Edit Role"),
                     new Claim("Delete Role", "Delete Role")
                });

                if (!res.Succeeded)
                {
                    return View("Error");

                }
            }

            var findevelopler = _userManager.FindByEmailAsync("developer@gmail.com");
            if (findevelopler.Result == null)
            {
                var user2 = new ApplicationUser { UserName = "developer@gmail.com", Email = "developer@gmail.com", ImagePath = "/images/StaticImages/Admin.png", EmployeeName = "developer" };
                var result2 = await _userManager.CreateAsync(user2, "Developer1@hamdard");

                var us2 = await _userManager.FindByEmailAsync("developer@gmail.com");

                us2.EmailConfirmed = true;
                await _userManager.UpdateAsync(us2);

                var res2 = await _userManager.AddToRoleAsync(us2, "Developer");

                await _userManager.AddClaimsAsync(us2, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role"),
                    new Claim("Edit Role", "Edit Role"),
                     new Claim("Delete Role", "Delete Role")
                });

                if (!res2.Succeeded)
                {
                    return View("Error");

                }
            }

            var findsuper = _userManager.FindByEmailAsync("superadmin@gmail.com");
            if (findsuper.Result == null)
            {
                var user3 = new ApplicationUser { UserName = "superadmin@gmail.com", Email = "superadmin@gmail.com", ImagePath = "/images/StaticImages/Admin.png", EmployeeName = "Super Admin" };
                var result3 = await _userManager.CreateAsync(user3, "Superadmin1@");

                var usr = await _userManager.FindByEmailAsync("superadmin@gmail.com");

                usr.EmailConfirmed = true;
                await _userManager.UpdateAsync(usr);

                var res3 = await _userManager.AddToRoleAsync(usr, "Super Admin");

              await _userManager.AddClaimsAsync(usr, new List<Claim>() {
                    new Claim("View Role", "View Role"),
                    new Claim("Insert Role", "Insert Role"),
                    new Claim("Edit Role", "Edit Role"),
                     new Claim("Delete Role", "Delete Role")
                });

                if (!res3.Succeeded)
                {
                    return View("Error");

                }
            }

            return View("Login");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
