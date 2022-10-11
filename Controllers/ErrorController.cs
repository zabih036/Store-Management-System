using Microsoft.AspNetCore.Mvc;

namespace GeneralSalesDb.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult ErrorHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.Message = "نوموړی صفحه شتون نه لرې";
                    ViewBag.Code = "404";
                    break;
                case 400:
                    ViewBag.Message = "نوموړی صفحه شتون نه لرې";
                    ViewBag.Code = "400";
                    break;
                case 501:
                    ViewBag.Message = " مهربانی له مخې پروژې جوړونکې سره اړیکه ونسئ";
                    ViewBag.Code = "501";
                    break;
                case 502:
                    ViewBag.Message = " مهربانی له مخې پروژې جوړونکې سره اړیکه ونسئ";
                    ViewBag.Code = "502";
                    break;
                case 503:
                    ViewBag.Message = " مهربانی له مخې پروژې جوړونکې سره اړیکه ونسئ";
                    ViewBag.Code = "503";
                    break;
                case 500:
                    ViewBag.Message = "مهربانی له مخې پروژې جوړونکې سره اړیکه ونسئ";
                    ViewBag.Code = "500";
                    break;
            }
            return View();
        }
    }
}