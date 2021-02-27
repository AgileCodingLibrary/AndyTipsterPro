using AndyTipsterPro.Helpers;
using AndyTipsterPro.Models;
using AndyTipsterPro.ViewModels;
using AndyTipsterPro.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayPal.v1.BillingAgreements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Globalization;

namespace AndyTipsterPro.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IConfiguration _configuration;
        private readonly PayPalHttpClientFactory _clientFactory;
        private readonly AppDbContext _dbContext;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IConfiguration configuration,
            PayPalHttpClientFactory clientFactory,
            AppDbContext dbContext

            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this._configuration = configuration;
            this._clientFactory = clientFactory;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (userHasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                var result = await userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("AddPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ViewModels.ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user,
                    model.CurrentPassword, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ViewModels.ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ViewModels.ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    var passwordResetLinkHtml = "<a href='" + passwordResetLink + "' >Reset Password</a>";

                    var sendGridKey = _configuration.GetValue<string>("SendGridApi");

                    //string email, string subject, string htmlContent)
                    await Emailer.SendEmail(user.Email, "Reset Password for AndyTipster",
                        "<p>Hi " + user.FirstName + " " + user.LastName + "</p>"
                        + "<p>A request to reset the password for your account has been made at AndyTipsterPro.com.</p>"
                        + "<p>You may now reset the password for your AndyTipsterPro account by clicking on this link.</p>"
                        + "<p>" + passwordResetLinkHtml + "</p>"
                        , sendGridKey);


                    logger.Log(LogLevel.Warning, passwordResetLink);

                    return View("ForgotPasswordConfirmation");
                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", new { controller = "Home" });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(ViewModels.RegisterViewModel model)
        {
            if (model.FirstName.Length > 80)
            {
                ModelState.AddModelError("First Name Too Long", "First Name is too long.");
            }

            if (model.FirstName.Contains("http"))
            {
                ModelState.AddModelError("First Name Invalid", "First Name contains Invalid characters.");
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,

                    //get send emails by default
                    SendEmails = true,

                    //no access by default to the tips
                    ManualElitePackageAccessExpiresAt = DateTime.Now.AddDays(-1),
                    ManualComboPackageAccessExpiresAt = DateTime.Now.AddDays(-1),
                    ManualUKRacingPackageAccessExpiresAt = DateTime.Now.AddDays(-1),

                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = user.Id, token = token }, Request.Scheme);

                    var confirmationLinkHtml = "<a href=" + confirmationLink + "> Click to Confirm your Email </a>";

                    var sendGridKey = _configuration.GetValue<string>("SendGridApi");

                    //string email, string subject, string htmlContent)
                    await Emailer.SendEmail(user.Email, "AndyTipster: Please confirm your email", "<p>" + confirmationLinkHtml + "</p>", sendGridKey);

                    logger.Log(LogLevel.Warning, confirmationLink);

                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                        "email, by clicking on the confirmation link we have emailed you. Missing Emails! Have you checked your Spam Folder?";
                    return View("Error");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            ViewModels.LoginViewModel model = new ViewModels.LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(ViewModels.LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                                    (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password,
                                        model.RememberMe, true);

                if (result.Succeeded)
                {

                    //check status of subscriptions
                    CheckUpdateUserSubscriptionDetails(user).Wait();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult>
        ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            ViewModels.LoginViewModel loginViewModel = new ViewModels.LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty,
                    $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                user = await userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginViewModel);
                }
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(
                                        info.LoginProvider, info.ProviderKey,
                                        isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);

                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                        new { userId = user.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                            "email, by clicking on the confirmation link we have emailed you. Check junk/spam/promotions folders as most of the time these generic subscription emails don’t filter to your main mail inbox";

                        return View("Error");
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support.";

                return View("Error");
            }
        }

        private async Task CheckUpdateUserSubscriptionDetails(ApplicationUser user)
        {

            List<UserSubscriptions> userSubs = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id && x.PayPalAgreementId != null).ToList();
            if (userSubs.Count() > 0)
            {

                //List<string> userSubscriptionIds = user.Subscriptions.Select(x => x.PayPalAgreementId).ToList();
                List<string> userSubscriptionIds = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id && x.PayPalAgreementId != null).Select(x => x.PayPalAgreementId).ToList();

                foreach (var Id in userSubscriptionIds)
                {
                    //get user subscription
                    var client = _clientFactory.GetClient();
                    AgreementGetRequest request = new AgreementGetRequest(Id);
                    BraintreeHttp.HttpResponse result = await client.Execute(request);
                    Agreement agreement = result.Result<Agreement>();

                    //get user subscription
                    UserSubscriptions userExistingSubscription = user.Subscriptions.Where(x => x.PayPalAgreementId == Id).FirstOrDefault();

                    //first time user is cancelling.
                    if (agreement.State == "Cancelled" && userExistingSubscription.State != "Cancelled")
                    {

                        //if this is the first cancellation.
                        if (userExistingSubscription.ExpiryDate.Year < 1995)
                        {
                            //set expiry date in the User Subscription to the next billing date.
                            //"last_payment_date": "2020-06-15T17:57:02Z",
                            // "next_billing_date": "2020-07-15T10:00:00Z",

                            string expiryDateAsString = agreement.AgreementDetails.NextBillingDate.Substring(0, 10);

                            DateTime expiryDate = DateTime.ParseExact(expiryDateAsString, "yyyy-MM-dd", null);

                            userExistingSubscription.ExpiryDate = expiryDate;

                            //update user subscription status.
                            userExistingSubscription.State = agreement.State;

                            //only send cancellation email when Paypal first sends cancellation confirmation.

                            //var cancellationEmailAlreadyBeenSent = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id && x.PayPalAgreementId == agreement.Id && x.State != "Cancelled").Any();
                            //if (!cancellationEmailAlreadyBeenSent)
                            //{
                            //    //Send user an email and let them know expiry date.
                            //    var confirmationHtml = $"<h2>As per your request, your Subscription <strong>{userExistingSubscription.Description}</strong> has been cancelled.</h2> <p>However, you can continue enjoy your access till your paid period expired on {userExistingSubscription.ExpiryDate}.</p>";
                            //    var sendGridKey = _configuration.GetValue<string>("SendGridApi");
                            //    await Emailer.SendEmail(user.Email, "AndyTipster Subscription, has been cancelled", confirmationHtml, sendGridKey);


                            //    //Send admin an email and let them know expiry date.
                            //    var confirmationAdminHtml = $"<h2>User {user.Email} has cancelled their subscription for <strong>{userExistingSubscription.Description}</strong>.</h2> <p>However, user has access till their paid period expired on {userExistingSubscription.ExpiryDate}.</p><p>An email confirmation has been sent to user on {user.Email}</p>";
                            //    await Emailer.SendEmail("andytipster99@outlook.com", "A user has cancelled a Subscription", confirmationAdminHtml, sendGridKey);
                            //}


                        }

                    }
                    if (agreement.State == "Cancelled" && userExistingSubscription.State == "Cancelled")
                    {
                        //check if subscription date has expired.
                        if (userExistingSubscription.ExpiryDate < DateTime.Now) // user subs expired, delete their subscription.
                        {
                            //delete Subscription.
                            var subsTobeDeleted = _dbContext.Subscriptions.Where(x => x.PayPalAgreementId == agreement.Id).FirstOrDefault();
                            if (subsTobeDeleted != null)
                            {
                                _dbContext.Subscriptions.Remove(subsTobeDeleted);
                            }

                            //delete user Subscription
                            var userSubsToBeDeleted = _dbContext.UserSubscriptions.Where(x => x.UserId == user.Id && x.State == "Cancelled" && x.PayPalAgreementId == agreement.Id).FirstOrDefault();
                            if (userSubsToBeDeleted != null)
                            {
                                _dbContext.UserSubscriptions.Remove(userSubsToBeDeleted);
                            }

                            _dbContext.SaveChanges();

                            //Send user an email and let them know subscription now has expired.                           
                            var expiredHtml = $"<h2>Your Subscription <strong>{userExistingSubscription.Description}</strong> has now expired.</h2>";
                            var sendGridKey = _configuration.GetValue<string>("SendGridApi");
                            await Emailer.SendEmail(user.Email, "Andy Tipster Subscription has expired.", expiredHtml, sendGridKey);

                            //Send admin an email and let them know expiry.
                            var expiredAdminHtml = $"<h2>User {user.Email} subscription for <strong>{userExistingSubscription.Description}</strong>. has now expired.</h2><p>An email confirmation has been sent to user on {user.Email}</p>";
                            await Emailer.SendEmail("andytipster99@outlook.com", $"{userExistingSubscription.PayerEmail} : Subscription has expired.", expiredAdminHtml, sendGridKey);
                        }

                    }

                    await userManager.UpdateAsync(user);
                }
            }
        }
    }

}
