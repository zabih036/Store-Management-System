using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.Model;
using Microsoft.AspNetCore.Authorization;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Admin Department")]
    public class AttendanceController : Controller
    {
        GeneralSalesDbContext db = new GeneralSalesDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        int depId = 0;
        public AttendanceController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult EmployeeAttendance()
        {
            return View();
        }

        public JsonResult FetchTeacherForAttendance()
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            var SelectedItemRecored = (from e in db.Employee
                                       where e.DepartmentId == depId
                                       select new
                                       {
                                           studentId = e.EmployeeId,
                                           image = e.Image,
                                           name = e.Name,
                                           fName = e.FatherName ?? "",
                                           assignedAttendance = db.EmployeeAttendance.Where(z => z.Date == DateTime.Now.Date
                                                   && z.EmployeeId == e.EmployeeId && z.DepartmentId == depId).FirstOrDefault(),
                                           attendance = db.EmployeeAttendance.Where(x => x.EmployeeId == e.EmployeeId && x.Date == DateTime.Now.Date && x.DepartmentId == depId).FirstOrDefault()

                                       }).OrderByDescending(r => r.studentId).ToList();
            return Json(SelectedItemRecored);
        }

        public async Task<JsonResult> AssignAttendance(int attStudentID, string attState, int saveOrEdit)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;
            if (saveOrEdit == 1)
            {
                EmployeeAttendance at = new EmployeeAttendance();
                at.EmployeeId = attStudentID;
                at.Status = attState;
                at.Date = DateTime.Now.Date;
                at.DepartmentId = depId;
                db.EmployeeAttendance.Add(at);
                await db.SaveChangesAsync();
            }
            else
            {
                var record = db.EmployeeAttendance.Where(x => x.EmployeeId == attStudentID && x.Date == DateTime.Now.Date && x.DepartmentId == depId).FirstOrDefault();
                record.EmployeeId = attStudentID;
                record.Status = attState;
                record.Date = DateTime.Now.Date;
                db.Entry(record).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
            }
            var data = new
            {
                state = attState,
                studentId = attStudentID
            };
            return Json(data);
        }

        public JsonResult attendanceAllTeacher(int year, int month)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var selectedClassAttendance = (from em in db.Employee
                                           where em.DepartmentId == depId
                                           select new
                                           {
                                               studentId = em.EmployeeId,
                                               image = em.Image,
                                               name = em.Name,
                                               fName = em.FatherName,
                                               totalCredit = db.EmployeeAttendance.Where(x => x.Date.Value.Year == year && x.DepartmentId == depId && x.Date.Value.Month == month && x.EmployeeId == em.EmployeeId).Count(),
                                               totalAttended = db.EmployeeAttendance.Where(x => x.Date.Value.Year == year && x.DepartmentId == depId && x.Date.Value.Month == month && x.Status == "حاضر" && x.EmployeeId == em.EmployeeId).Count(),
                                               totalLeave = db.EmployeeAttendance.Where(x => x.Date.Value.Year == year && x.DepartmentId == depId && x.Date.Value.Month == month && x.Status == "رخصت" && x.EmployeeId == em.EmployeeId).Count(),
                                               totalAbsent = db.EmployeeAttendance.Where(x => x.Date.Value.Year == year && x.DepartmentId == depId && x.Date.Value.Month == month && x.Status == "غیر حاضر" && x.EmployeeId == em.EmployeeId).Count(),
                                               year = year,
                                               month = month
                                           }).ToList();
            return Json(selectedClassAttendance);
        }

        public JsonResult attendanceOfSelectedTeacher(int studentIdForAttendance, int yearForDetaile, int monthForDetaile)
        {
            depId = _userManager.GetUserAsync(User).Result.DepartmentId;

            var attendance = (from at in db.EmployeeAttendance.Where(x => x.EmployeeId == studentIdForAttendance && x.DepartmentId == depId && x.Date.Value.Year == yearForDetaile && x.Date.Value.Month == monthForDetaile)
                              select new
                              {
                                  date = at.Date.ToString(),
                                  status = at.Status
                              });
            return Json(attendance);
        }

        public IActionResult MyAttendance()
        {
            return View();
        }
    }
}
