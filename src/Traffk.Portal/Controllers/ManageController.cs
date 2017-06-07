﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Permissions;
using Traffk.Portal.Models.ManageViewModels;
using Traffk.Portal.Permissions;
using TraffkPortal.Models.ManageViewModels;
using TraffkPortal.Services;
using TraffkPortal.Services.Sms;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class ManageController : BasePageController
    {
        public const string Name = "Manage";

        public class ActionNames
        {
            public const string Index = "Index";
            public const string PhoneNumber = "AddPhoneNumber";

        }

        public enum PageKeys
        {
            Basics,
            PortalSettings,
            RegistrationSettings,
            SystemCommunications,
        }


        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IEmailSender EmailSender;
        private readonly ISmsSender SmsSender;
        private readonly bool IsSigninPersistent;

        public ManageController(
            TraffkTenantModelDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            CurrentContextServices current,
            ILogger logger
            )
            : base(AspHelpers.MainNavigationPageKeys.Manage, db, current, logger)
        {

            UserManager = userManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
            SmsSender = smsSender;
            IsSigninPersistent = Startup.IsSigninPersistent;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : message == ManageMessageId.RemovePhoneTwoFactorError ? "Your phone number cannot be removed, it is being used for two-factor authentication.  Please disable two-factor authentication before removing."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }
            var model = new IndexViewModel
            {
                HasPassword = await UserManager.HasPasswordAsync(user),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(user),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(user),
                Logins = await UserManager.GetLoginsAsync(user),
                BrowserRemembered = await SignInManager.IsTwoFactorClientRememberedAsync(user)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        {
            ManageMessageId? message = ManageMessageId.Error;
            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                var result = await UserManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
                if (result.Succeeded)
                {

                    await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                    message = ManageMessageId.RemoveLoginSuccess;
                }
            }
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }
            model.PhoneNumber = GetRawPhoneNumber(model.PhoneNumber);
            model.PhoneNumber = ParseCountryCode(model.PhoneNumber);

            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await SmsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                if (String.IsNullOrEmpty(user.PhoneNumber) || !user.PhoneNumberConfirmed)
                {
                    return RedirectToAction(nameof(AddPhoneNumber), "Manage");
                }


                await UserManager.SetTwoFactorEnabledAsync(user, true);
                await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                Logger.Information("User enabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                await UserManager.SetTwoFactorEnabledAsync(user, false);
                await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                Logger.Information("User disabled two-factor authentication.");
            }
            return RedirectToAction(nameof(Index), "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }

            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
            // Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                var result = await UserManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {

                    await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            // If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Failed to verify phone number");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoneNumber()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                if (user.TwoFactorEnabled)
                {
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneTwoFactorError });
                }


                var result = await UserManager.SetPhoneNumberAsync(user, null);
                if (result.Succeeded)
                {

                    await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
                }
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {

                    await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                    Logger.Information("User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {

                var result = await UserManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {

                    await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //GET: /Manage/ManageLogins
        [HttpGet]
        public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }


            var userLogins = await UserManager.GetLoginsAsync(user);
            var otherLogins = SignInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");

            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, UserManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }

            var info = await SignInManager.GetExternalLoginInfoAsync(await UserManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(ManageLogins), new { Message = ManageMessageId.Error });
            }

            var result = await UserManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(ManageLogins), new { Message = message });
        }

        [HttpGet]
        [ApiAuthorize(ApiNames.Base)]
        public async Task<IActionResult> GetApiKey()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }

            var model = new GenerateApiKeyViewModel {ApiKey = user.Settings.ApiKey};

            return View(model);
        }

        [HttpPost]
        [ApiAuthorize(ApiNames.Base)]
        public async Task<IActionResult> GetApiKey(GenerateApiKeyViewModel model)
        {
            ModelState.Clear();

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }

            var apiKey = Guid.NewGuid();
            user.Settings.ApiKey = apiKey;
            await UserManager.UpdateAsync(user);

            model.ApiKey = user.Settings.ApiKey;
            return View("GetApiKey", model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            RemovePhoneTwoFactorError,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {

            return UserManager.GetUserAsync(HttpContext.User);
        }

        private static string GetRawPhoneNumber(string phone)
        {
            return new string(phone.Where(char.IsDigit).ToArray());
        }

        private static string ParseCountryCode(string phone)
        {
            var length = phone.Length;
            if (length == 10)
            {
                return "1" + phone;
            }

            return phone;
        }

        #endregion
    }
}
