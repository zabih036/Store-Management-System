using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GeneralSalesDb.Models;
using System.Collections.Generic;
using System.IO;

namespace GeneralSalesDb.Controllers
{
    [Authorize(Roles = "Admin Department")]
    public class BackupController : Controller
    {
        // GET: Backup

        SQLLocalBackup backup = new SQLLocalBackup();
        public ActionResult Index()
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

        public IActionResult Create(string AlocalPath)
        {
            backup.DoLocalBackup(AlocalPath);
            TempData["updated"] = " بیک اپ واخیستل شو";
            return RedirectToAction("Index", "Backup");

        }

        public IActionResult Restoredb(string Restorepath)
        {
            backup.Restoredb(Restorepath);
            TempData["updated"] = " ډاټابیس ریزټور شو";
            return RedirectToAction("Index", "Backup");

        }

        public JsonResult List()
        {
            List<string> lst = new List<string>();
            string[] AremoteTempPath = Directory.GetFiles("C:\\Backup");
            foreach (string Path in AremoteTempPath)
            {
                lst.Add(Path);
            }
            return Json(lst);
        }

        public IActionResult DeleteBackup(string path)
        {
            path = path.Replace("/", "\\");
           
            System.IO.File.Delete(path);

            TempData["updated"] = " بیک حذف شو";
            return RedirectToAction("Index", "Backup");
        }

        public FileResult DownloadBackup(string path)
        {
          
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, path.Replace("C:\\Backup\\", ""));
        }

    }
}