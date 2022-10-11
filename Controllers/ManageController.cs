using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GeneralSalesDb.Models;
using GeneralSalesDb.Models.ManageViewModels;
using GeneralSalesDb.Services;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace GeneralSalesDb.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {

            if (TempData["updated"] != null)
            {
                ViewBag.Alert = TempData["updated"];
            }
            else
            {
                ViewBag.Alert = "empty";
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                //return RedirectToAction(nameof(SetPassword));
            }

            //var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(/*model*/);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"کارمند په دی ایدی نشته '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["updated"] = " ستاسو پټ نوم بدل شو.";

            return RedirectToAction(nameof(ChangePassword));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                if (error.Description.Equals("Incorrect password."))
                {
                    error.Description = "زوړ پټ نوم مو غلط دی";
                }
                else
                {
                    error.Description = @" پټ نوم باید یو سیمبول یو غټ حروف او یو عدد ولرې.";
                }
                ModelState.AddModelError(string.Empty, error.Description);
                break;
            }
        }

        //private string FormatKey(string unformattedKey)
        //{
        //    var result = new StringBuilder();
        //    int currentPosition = 0;
        //    while (currentPosition + 4 < unformattedKey.Length)
        //    {
        //        result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
        //        currentPosition += 4;
        //    }
        //    if (currentPosition < unformattedKey.Length)
        //    {
        //        result.Append(unformattedKey.Substring(currentPosition));
        //    }

        //    return result.ToString().ToLowerInvariant();
        //}

        //private string GenerateQrCodeUri(string email, string unformattedKey)
        //{
        //    return string.Format(
        //        AuthenticatorUriFormat,
        //        _urlEncoder.Encode("GeneralSalesDb"),
        //        _urlEncoder.Encode(email),
        //        unformattedKey);
        //}

        //private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        //{
        //    var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        //    if (string.IsNullOrEmpty(unformattedKey))
        //    {
        //        await _userManager.ResetAuthenticatorKeyAsync(user);
        //        unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        //    }

        //    model.SharedKey = FormatKey(unformattedKey);
        //    model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        //}

        #endregion
    }
}
