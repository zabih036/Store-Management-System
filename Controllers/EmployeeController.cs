using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.ViewModels;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GeneralSalesDb.Models.Model;

namespace GeneralSalesDb.Controllers
{

    public class EmployeeController : Controller
    {

        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly IConfiguration configuration;
        int depId = 0;
        [Obsolete]
        public EmployeeController(IWebHostEnvironment hostingEnvironment,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            this.hostingEnvironment = hostingEnvironment;
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            configuration = builder.Build();

        }

        [HttpGet]
        [Authorize(Roles = "Admin Department")]
        public IActionResult Employee()
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

        public JsonResult LoadEmployee()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var rec = db.Employee.Where(x => x.DepartmentId == depId).ToList().OrderByDescending(d => d.EmployeeId);
            return Json(rec);
        }

        [Authorize(Roles = "Admin Department")]
        public IActionResult LoadEmployeeSalary()
        {
           
            try
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                var rec = (from e in db.Employee
                           where e.DepartmentId == depId
                           select new
                           {
                               id = e.EmployeeId,
                               salaryId = db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.SalaryId).FirstOrDefault(),
                               imagePath = e.Image,
                               name = e.Name,
                               currencyId = e.CurrencyId,
                               absentCount = db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر" && x.Date.Value.Year == DateTime.Now.Year && x.Date.Value.Month == DateTime.Now.Month).Count(),
                               absentAmount = ((e.Salary / daysInMonth ) * db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر" && x.Date.Value.Year == DateTime.Now.Year && x.Date.Value.Month == DateTime.Now.Month).Count()),
                               overtime = db.OverTime.Where(x => x.EmployeeId == e.EmployeeId && x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).Sum(x => x.OverTimeHours) == 0 ? 0.0 : db.OverTime.Where(x => x.EmployeeId == e.EmployeeId && x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).Sum(x => x.OverTimeHours),
                               phone = e.Phone,
                               hireDate = Convert.ToDateTime(e.HireDate).ToShortDateString(),
                               email = e.Email,
                               address = e.Address,
                               salary = e.Salary,
                               totalAbsentAmount = Convert.ToDouble((e.Salary / daysInMonth) * db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر").Count()),
                               totalOvertimeAmount = Convert.ToDouble(db.OverTime.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.OverTimeHours) == 0 ? 0.0 : db.OverTime.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.OverTimeHours)),
                               lastPaid = Convert.ToDouble(db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.PaidAmount).FirstOrDefault()),
                               totalMonths = Math.Floor(DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays / daysInMonth),
                               totalPaid = Convert.ToDouble(db.SalaryPayment.Where(p => p.EmployeeId == e.EmployeeId).Sum(d => d.PaidAmount).ToString()),
                               totalSalary = e.Salary * Math.Floor(DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays / daysInMonth),
                               totalRemain = (Convert.ToDouble(e.Salary * Math.Floor((DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays)) / daysInMonth)) - db.SalaryPayment.Where(p => p.EmployeeId == e.EmployeeId).Sum(d => d.PaidAmount),
                               paidDate = db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.PaidDate).FirstOrDefault()
                           }).ToList().GroupBy(x => x.id).Select(r => r.LastOrDefault());
                return Json(rec);

                //depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                //var rec = (from e in db.Employee
                //           where e.DepartmentId == depId
                //           select new
                //           {
                //               id = e.EmployeeId,
                //               salaryId = db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.SalaryId).FirstOrDefault(),
                //               imagePath = e.Image,
                //               name = e.Name,
                //               absentCount = db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر" && x.Date.Value.Year == DateTime.Now.Year && x.Date.Value.Month == DateTime.Now.Month).Count(),
                //               absentAmount = ((e.Salary / 30) * db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر" && x.Date.Value.Year == DateTime.Now.Year && x.Date.Value.Month == DateTime.Now.Month).Count()),
                //               overtime = db.OverTime.Where(x => x.EmployeeId == e.EmployeeId && x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).Sum(x => x.OverTimeHours) == 0 ? 0.0 : db.OverTime.Where(x => x.EmployeeId == e.EmployeeId && x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).Sum(x => x.OverTimeHours),
                //               phone = e.Phone,
                //               hireDate = Convert.ToDateTime(e.HireDate).ToShortDateString(),
                //               email = e.Email,
                //               province = e.Province,
                //               salary = e.Salary,
                //               paidAmount = (Convert.ToDouble(db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.PaidAmount).FirstOrDefault()) - Convert.ToDouble((e.Salary / 30) * db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Status == "غیر حاضر").Count())) + Convert.ToDouble( db.OverTime.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.OverTimeHours) == 0 ? 0.0 : db.OverTime.Where(x => x.EmployeeId == e.EmployeeId).Sum(x => x.OverTimeHours)),
                //               totalMonths = Math.Floor(DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays / 30),
                //               totalPaid = Convert.ToDouble(db.SalaryPayment.Where(p => p.EmployeeId == e.EmployeeId).Sum(d => d.PaidAmount).ToString()) ,
                //               totalSalary = e.Salary * Math.Floor(DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays / 30),
                //               totalRemain = (Convert.ToDouble(e.Salary * Math.Floor((DateTime.Now.Date.Subtract(Convert.ToDateTime(e.HireDate)).TotalDays)) / 30)) - db.SalaryPayment.Where(p => p.EmployeeId == e.EmployeeId).Sum(d => d.PaidAmount),
                //               paidDate = db.SalaryPayment.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.SalaryId).Select(r => r.PaidDate).FirstOrDefault()
                //           }).ToList().GroupBy(x => x.id).Select(r => r.LastOrDefault());
                //return Json(rec);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.StackTrace);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin Department")]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> EmployeeRegistration(AllViewModel model, [FromForm] IFormFile img)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;
                string uploadPath = "";
                string upload = "";
                if (img == null)
                {
                    if (model.employee.EmployeeId == 0)
                        upload = "/images/StaticImages/browse.png";
                }
                else
                {

                    var fileName = model.employee.Email + Guid.NewGuid().ToString().Replace("_", "") + Path.GetExtension(img.FileName);
                    upload = Path.Combine("Images/EmployeeImage/", fileName);
                    uploadPath = Path.Combine(hostingEnvironment.WebRootPath, upload);
                    img.CopyTo(new FileStream(uploadPath, FileMode.Create));
                    upload = "/" + upload;

                }

                if (model.employee.EmployeeId != 0)
                {
                    var user1 = await _userManager.FindByEmailAsync(model.employee.Email);
                    string ro = "";
                    if (user1 != null)
                    {
                        var role = await _userManager.GetRolesAsync(user1);
                        ro = role[0].ToString();
                        var logeduser = await _userManager.GetUserAsync(User);
                        var rol = await _userManager.GetRolesAsync(logeduser);
                        string r = rol[0].ToString();
                        if (ro == "Admin Department")
                        {
                            if (r == "Admin Department")
                            {
                                var rec1 = await db.Employee.FindAsync(model.employee.EmployeeId);
                                if (rec1 == null)
                                {
                                    return View("Error");
                                }
                                rec1.Name = model.employee.Name;
                                rec1.FatherName = model.employee.FatherName;
                                rec1.Address = model.employee.Address;
                                rec1.TazkiraNo = model.employee.TazkiraNo;
                                rec1.Job = model.employee.Job;
                                rec1.CurrencyId = model.employee.CurrencyId;
                                rec1.Phone = model.employee.Phone.ToString();
                                rec1.Email = model.employee.Email;
                                rec1.HireDate = model.employee.HireDate;
                                rec1.Salary = model.employee.Salary;

                                var user2 = await _userManager.FindByEmailAsync(model.employee.oldEmail);
                                var department1 = db.Department.Where(x => x.Email == model.employee.oldEmail).FirstOrDefault();

                                if (img == null && model.employee.defalut != null)
                                {
                                    rec1.Image = model.employee.defalut;
                                }
                                else { rec1.Image = upload; }


                                db.Entry(rec1).State = EntityState.Modified;
                                if (img != null && model.employee.defalut == "0")
                                {
                                    db.Entry(rec1).Property(d => d.Image).IsModified = true;
                                    if (user2 != null)
                                        user2.ImagePath = upload;
                                }
                                else if (model.employee.defalut != "0")
                                {
                                    db.Entry(rec1).Property(d => d.Image).IsModified = true;
                                    if (user2 != null)
                                        user2.ImagePath = model.employee.defalut;
                                }

                                if (department1 != null)
                                {
                                    department1.Email = model.employee.Email;

                                    db.Department.Update(department1);

                                    await db.SaveChangesAsync();
                                }

                                if (user2 != null)
                                {


                                    var result = await _userManager.SetEmailAsync(user2, model.employee.Email);
                                    await _userManager.SetUserNameAsync(user2, model.employee.Email);
                                    if (result.Succeeded)
                                    {
                                        user2.EmployeeName = model.employee.Name;
                                        user2.PhoneNumber = model.employee.Phone;
                                        user2.EmailConfirmed = true;
                                        await _userManager.UpdateAsync(user2);
                                    }
                                    else
                                    {
                                        return View("Error");
                                    }

                                }

                                await db.SaveChangesAsync();

                                TempData["updated"] = " ریکارډ تغیر شو";

                                return RedirectToAction("Employee", "Employee");
                            }
                            else
                            {
                                return View("Error");
                            }

                        }
                    }

                    var rec = await db.Employee.FindAsync(model.employee.EmployeeId);
                    if (rec == null)
                    {
                        return View("Error");
                    }
                    rec.Name = model.employee.Name;
                    rec.FatherName = model.employee.FatherName;
                    rec.Address = model.employee.Address;
                    rec.TazkiraNo = model.employee.TazkiraNo;
                    rec.Job = model.employee.Job;
                    rec.CurrencyId = model.employee.CurrencyId;
                    rec.Phone = model.employee.Phone.ToString();
                    rec.Email = model.employee.Email;
                    rec.HireDate = model.employee.HireDate;
                    rec.Salary = model.employee.Salary;

                    var user = await _userManager.FindByEmailAsync(model.employee.oldEmail);
                    var department = db.Department.Where(x => x.Email == model.employee.oldEmail).FirstOrDefault();

                    if (img == null && model.employee.defalut != null)
                    {
                        rec.Image = model.employee.defalut;
                    }
                    else { rec.Image = upload; }


                    db.Entry(rec).State = EntityState.Modified;
                    if (img != null && model.employee.defalut == "0")
                    {
                        db.Entry(rec).Property(d => d.Image).IsModified = true;
                        if (user != null)
                            user.ImagePath = upload;
                    }
                    else if (model.employee.defalut != "0")
                    {
                        db.Entry(rec).Property(d => d.Image).IsModified = true;
                        if (user != null)
                            user.ImagePath = model.employee.defalut;
                    }

                    if (department != null)
                    {
                        department.Email = model.employee.Email;

                        db.Department.Update(department);

                        await db.SaveChangesAsync();
                    }
                    if (user != null)
                    {
                        var result = await _userManager.SetEmailAsync(user, model.employee.Email);
                        await _userManager.SetUserNameAsync(user, model.employee.Email);
                        if (result.Succeeded)
                        {
                            user.EmployeeName = model.employee.Name;
                            user.PhoneNumber = model.employee.Phone;
                            user.EmailConfirmed = true;
                            await _userManager.UpdateAsync(user);
                        }
                        else
                        {
                            return View("Error");
                        }
                    }

                    await db.SaveChangesAsync();

                    TempData["updated"] = " ریکارډ تغیر شو";

                    return RedirectToAction("Employee", "Employee");

                }
                else
                {
                    try
                    {
                        Employee it = new Employee()
                        {
                            Name = model.employee.Name,
                            FatherName = model.employee.FatherName,
                            Address = model.employee.Address,
                            TazkiraNo = model.employee.TazkiraNo,
                            Job = model.employee.Job,
                            CurrencyId = model.employee.CurrencyId,
                            Phone = model.employee.Phone.ToString(),
                            Email = model.employee.Email,
                            HireDate = Convert.ToDateTime(model.employee.HireDate.ToShortDateString()).Date,
                            Salary = model.employee.Salary,
                            Image = upload,
                            DepartmentId = depId
                        };
                        db.Employee.Add(it);

                        await db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    TempData["updated"] = " ریکارډ ذخیره شو";

                    return RedirectToAction("Employee", "Employee");
                }

            }
            return View("Error");

        }

        [Authorize(Roles = "Admin Department")]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteEmployee(AllViewModel model)
        {
            if (model.employee.EmployeeId != 0)
            {
                var comp = await db.Employee.FindAsync(model.employee.EmployeeId);
                if (comp == null)
                {
                    return View("Error");
                }

                try
                {
                    var recs = db.Note.Where(d => d.UserId == model.employee.Email);
                    if (recs != null)
                        db.Note.RemoveRange(recs);
                    var account = await _userManager.FindByEmailAsync(model.employee.Email);
                    if (account != null)
                    {
                        var result = await _userManager.DeleteAsync(account);
                        if (!result.Succeeded)
                        {
                            return View("Error");
                        }
                    }

                    db.Employee.Remove(comp);
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

        public IActionResult IsEmployeeEmailExist(AllViewModel model)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            if (model.employee.EmployeeId != 0)
            {
                var em = db.Employee.Where(d => d.Email == model.employee.Email && d.EmployeeId != model.employee.EmployeeId && d.DepartmentId == depId).ToList().Count();
                if (em > 0)
                {
                    return Json(false);

                }
                else
                {
                    return Json(true);
                }
            }
            var rec = db.Employee.Any(d => d.Email == model.employee.Email && d.DepartmentId == depId);

            if (!rec)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        [Authorize(Roles = "Admin Department")]
        [Authorize(Policy = "InsertRolePolicy")]
        public async Task<IActionResult> AddEmployeeSalary(AllViewModel model,int currency)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                if (model.salary.SalaryId != 0)
                {
                    var rec = await db.SalaryPayment.FindAsync(model.salary.SalaryId);
                    if (rec == null)
                    {
                        return View("Error");
                    }

                    rec.PaidAmount = model.salary.PaidAmount;

                    db.SalaryPayment.Update(rec);

                    var rec4 = db.CashInHand.Where(x => x.SalaryId == model.salary.SalaryId).FirstOrDefault();

                    rec4.Debit = model.salary.PaidAmount;
                    rec4.Date = DateTime.Now.Date;

                    db.CashInHand.Update(rec4);

                    await db.SaveChangesAsync();

                    return Json("ریکارډ تغیر شو");

                }
                else
                {
                    try
                    {

                        var email = _userManager.GetUserAsync(User).Result.Email;

                        SalaryPayment it = new SalaryPayment()
                        {
                            CurrencyId = model.salary.CurrencyId,
                            PaidAmount = model.salary.PaidAmount,
                            PaidDate = DateTime.Now.Date,
                            EmployeeId = model.salary.EmployeeId,
                            Category = model.salary.Category,
                            PaidBy = email,
                            DepartmentId = depId
                        };

                        db.SalaryPayment.Add(it);
                        await db.SaveChangesAsync();

                        CashInHand ch = new CashInHand()
                        {
                            Debit = model.salary.PaidAmount,
                            Credit = 0,
                            Date = DateTime.Now.Date,
                            CurrencyId = model.salary.CurrencyId,
                            Description = "معاش تادیه",
                            SalaryId = it.SalaryId,
                            DepartmentId = depId,
                        };
                        db.CashInHand.Add(ch);
                        await db.SaveChangesAsync();

                        return Json("نوموړی کارمند ته معاش ورسول شو");
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.StackTrace);
                    }
                }

            }
            return View("Error");

        }

        [Authorize(Roles = "Admin Department")]
        public IActionResult LoadEmployeeAllPayments(AllViewModel model)
        {
            if (model.employee.EmployeeId != 0)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var comp = (from s in db.SalaryPayment
                            where s.EmployeeId == model.employee.EmployeeId && s.DepartmentId == depId
                            select new
                            {
                                paidDate = Convert.ToDateTime(s.PaidDate).Date.ToShortDateString(),
                                paidAmount = s.PaidAmount
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

                    return Json("نوموړی کارمند ته معاش نه دی ورکړل شویږ");
                }
            }
            return NotFound();

        }
      
       
        [HttpGet]
        [Authorize(Roles = "Admin Department,Purchase Department,Sale Department,Finance Department,Stock Department")]
        public IActionResult Notes()
        {
            return View();
        }

        public async Task<JsonResult> AddNote(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                var email = _userManager.GetUserAsync(User).Result.Email;


                if (model.NoteId == 0)
                {
                    Note nt = new Note()
                    {
                        Note1 = model.Note,
                        TargetDate = Convert.ToDateTime(model.TargetDate.ToShortDateString()),
                        RemindDate = Convert.ToDateTime(model.RemindDate.ToShortDateString()),
                        UserId = email,
                        NoteStatus = "Active",
                        DepartmentId = depId
                    };
                    db.Note.Add(nt);
                    await db.SaveChangesAsync();
                    return Json("نوټ ذخیره شو!!!");
                }
                else
                {
                    Note targetedNote = db.Note.Where(x => x.NoteId == model.NoteId).FirstOrDefault();
                    //   Note nts = new Note();
                    targetedNote.NoteId = model.NoteId;
                    targetedNote.Note1 = model.Note;
                    targetedNote.TargetDate = Convert.ToDateTime(model.TargetDate.ToShortDateString());
                    targetedNote.RemindDate = Convert.ToDateTime(model.RemindDate.ToShortDateString());
                    db.Entry(targetedNote).Property(x => x.NoteStatus).IsModified = false;
                    db.Entry(targetedNote).Property(x => x.UserId).IsModified = false;
                    db.Entry(targetedNote).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Json("نوټ تغیر شو!!!");
                }
            }


            return Json("");
        }

        public JsonResult FetchNotes()
        {
            var email = _userManager.GetUserAsync(User).Result.Email;
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var AcitveNotes = (from N in db.Note
                               where N.UserId == email && N.NoteStatus == "Active" && N.DepartmentId == depId

                               select new
                               {
                                   noteid = N.NoteId,
                                   note = N.Note1,
                                   targetdate = N.TargetDate,
                                   targetdateShow = Convert.ToDateTime(N.TargetDate).ToShortDateString(),
                                   reminddate = N.RemindDate,
                                   reminddateShow = Convert.ToDateTime(N.RemindDate).ToShortDateString(),
                                   notestatus = N.NoteStatus,

                               }).ToList();

            return Json(AcitveNotes);
        }

        public JsonResult DeleteNote(Note note)
        {
            var record = db.Note.Find(note.NoteId);
            db.Note.Remove(record);
            db.SaveChanges();
            return Json("نوټ حذف شو !!!");
        }

        public JsonResult countActiveNotes()
        {
            var email = _userManager.GetUserAsync(User).Result.Email;
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var activeNotes = db.Note.Where(x => x.RemindDate <= DateTime.Now && x.TargetDate >= DateTime.Now && x.UserId == email && x.DepartmentId == depId).ToList();
            return Json(activeNotes.Count());
        }

        public JsonResult LoadActiveNotes()
        {
            var email = _userManager.GetUserAsync(User).Result.Email;
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var activeNotes = from note in db.Note.Where(x => x.RemindDate <= DateTime.Now && x.TargetDate >= DateTime.Now && x.UserId == email && x.DepartmentId == depId)
                              select new
                              {
                                  note = note.Note1,
                                  targetDate = Convert.ToDateTime(note.TargetDate).ToShortDateString()
                              };
            return Json(activeNotes);
        }

        [Authorize(Policy = "InsertRolePolicy")]
        [Authorize(Roles = "Admin Department,Finance Department")]
        public async Task<IActionResult> AddOverTime(AllViewModel model)
        {
            if (ModelState.IsValid)
            {
                depId = _userManager.GetUserAsync(User).Result.DepartmentId;

                OverTime ot = new OverTime
                {
                    OverTimeHours = model.overTime.Amount,
                    EmployeeId = model.overTime.EmployeeId,
                    Date = model.overTime.Date,
                    Description = model.overTime.Details,
                    DepartmentId = depId
                };
                db.OverTime.Add(ot);
                await db.SaveChangesAsync();
                return Json(" کارمند ته اضافه کارې ورسول شو");
            }
            return View("Error");
        }
    }
}