using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RevolutionaryStuff.Core;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Traffk.Bal.Communications;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using TraffkPortal.Models.AccountViewModels;
using TraffkPortal.Services;
using TraffkPortal.Services.Sms;
using ILogger = Serilog.ILogger;

namespace TraffkPortal.Controllers
{
    [Authorize]
    public class AccountController : BasePageController
    {
        public static readonly string Name = "Account";

        private readonly UserManager<ApplicationUser> UserManager;
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly IEmailSender EmailSender;
        private readonly ISmsSender SmsSender;
        private readonly bool IsSigninPersistent;

        private static class SignInResultStrings
        {
            public static readonly string Failed = "Failed";
        }

        private static class MessageStrings
        {
            public const string EmailConfirmed = "Your email address has been verified.";
            public const string ConfirmedEmailRequired = "You must have a verified email to log in.";
            public const string UserLockedOut = "Your account is locked.";
            public const string IncorrectUsernameOrPassword = "You've entered an incorrect username or password.";
            public const string UserSelfRegistrationDisabled = "User self registration is currently disabled.";
            public const string CheckEmailForVerification =
                AccountCreated + " " + "Please check your email to verify your account.";
            public const string AccountCreated = "Your new account has been created.";
        }

        public AccountController(
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            //We don't want any loops
            if (!(
                returnUrl.Contains(nameof(InactivityLogOff), true) ||
                returnUrl.Contains(nameof(LogOff), true) ||
                returnUrl.Contains(nameof(Login), true)))
            {
                ViewData["ReturnUrl"] = returnUrl;
            }
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
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await UserManager.FindByNameAsync(model.Email);
                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, IsSigninPersistent /*model.RememberMe*/, lockoutOnFailure: false);
                try
                {
                    if (result.Succeeded)
                    {
                        if (Current.Tenant.TenantSettings.RequiresEmailAccountValidation)
                        {
                            if (user != null)
                            {
                                if (!await UserManager.IsEmailConfirmedAsync(user))
                                {
                                    ModelState.AddModelError(string.Empty, MessageStrings.ConfirmedEmailRequired);
                                    return View(model);
                                }
                            }
                        }

                        Logger.Information("User logged in.");

                        Uri u;
                        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out u))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    }

                    if (result.ToString() == SignInResultStrings.Failed)
                    {
                        ModelState.AddModelError(string.Empty, MessageStrings.IncorrectUsernameOrPassword);
                        return View(model);
                    }

                    if (result.RequiresTwoFactor || Current.Tenant.TenantSettings.RequiresTwoFactorAuthentication)
                    {
                        return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    }

                    if (result.IsLockedOut)
                    {
                        Logger.Information(MessageStrings.UserLockedOut);
                        return View("Lockout");
                    }

                    ModelState.AddModelError(string.Empty, MessageStrings.IncorrectUsernameOrPassword);
                    return View(model);
                }
                finally
                {
                    if (user != null)
                    {
                        user.LogSignInAttempt(result);
                        await Rdb.SaveChangesAsync();
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (!Current.Application.AppSettings.Registration.UsersCanSelfRegister)
            {
                throw new NotNowException(MessageStrings.UserSelfRegistrationDisabled);
            }
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    if (Current.Tenant.TenantSettings.RequiresEmailAccountValidation)
                    {
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action(nameof(ConfirmEmail), Name, new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        await EmailSender.SendEmailCommunicationAsync(
                            SystemCommunicationPurposes.UserAccountVerification,
                            CommunicationModelFactory.CreateCallbackUrlModel(callbackUrl),
                            model.Email);
                        SetToast(MessageStrings.CheckEmailForVerification);
                    }
                    else
                    {
                        await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                        SetToast(MessageStrings.AccountCreated);
                    }

                    Logger.Information("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public IActionResult KeepAlive() => NoContent();

        public async Task<IActionResult> InactivityLogOff()
        {
            await SignInManager.SignOutAsync();
            Logger.Information("User logged out.");
            var returnUrl = HttpContext.Request.Headers[WebHelpers.HeaderStrings.Referer][0];
            return RedirectToAction(nameof(Login), new { returnUrl = returnUrl });
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await SignInManager.SignOutAsync();
            Logger.Information("User logged out.");
            return RedirectToHome();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), Name, new { ReturnUrl = returnUrl });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: IsSigninPersistent);
            if (result.Succeeded)
            {
                Logger.Information("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await SignInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: IsSigninPersistent);
                        Logger.Information("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId = null, string code = null)
        {
            Requires.NonNull(userId, nameof(userId));
            Requires.NonNull(code, nameof(code));

            var user = await UserManager.FindByIdAsync(userId);
            Requires.NonNull(user, nameof(user));

            var result = await UserManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                SetToast(MessageStrings.EmailConfirmed);
            }

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)// || !(await UserManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await UserManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), Name, new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await EmailSender.SendEmailCommunicationAsync(
                    SystemCommunicationPurposes.UserPasswordReset,
                    CommunicationModelFactory.CreateCallbackUrlModel(callbackUrl),
                    model.Email,
                    null,
                    user.ContactId);
                return View(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/AcceptInvitation
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptInvitation(string userId, string code = null)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null || code==null) return NotFound();
            var model = new ResetPasswordViewModel
            {
                Email = user.Email,
                Code = code,
            };
            return View(model);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptInvitation(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return RedirectToHome();
            }
            var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId = null, string code = null)
        {
            if (code == null)
            {
                return ErrorResult();
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var model = new ResetPasswordViewModel
                {
                    Email = user.Email
                };
                return View(model);
            }
            return ErrorResult();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), Name);
            }
            var result = await UserManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), Name);
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }

            // Generate the token and send it
            var code = await UserManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return ErrorResult();
            }

            if (model.SelectedProvider == "Email")
            {
                await EmailSender.SendEmailCommunicationAsync(
                    SystemCommunicationPurposes.UserTwoFactorLoginCode,
                    CommunicationModelFactory.CreateSimpleCodeModel(code),
                    await UserManager.GetEmailAsync(user));
            }
            else if (model.SelectedProvider == "Phone")
            {
                await SmsSender.SendSmsCommunicationAsync(
                    SystemCommunicationPurposes.UserTwoFactorLoginCode,
                    CommunicationModelFactory.CreateSimpleCodeModel(code),
                    await UserManager.GetPhoneNumberAsync(user));
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return ErrorResult();
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, IsSigninPersistent /*model.RememberMe*/, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                Logger.Warning("User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return UserManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToHome();
            }
        }

        #endregion
    }
}
