using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using AndyTipsterPro.ViewModels;
using AndyTipsterPro.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AndyTipsterPro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger,
                              AppDbContext db,
                              IConfiguration configuration,
                              UserManager<ApplicationUser> userManager)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            _db = db;
            _configuration = configuration;
            this._userManager = userManager;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _db.LandingPages.FirstOrDefault();

            return View(model);
            //return View("HomeMaster");
        }

        [AllowAnonymous]
        public ViewResult About()
        {
            var model = _db.Abouts.FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }

            return View("Index");
        }

        [AllowAnonymous]
        public ViewResult Twitter()
        {

            return View();
        }

        [AllowAnonymous]
        public ViewResult Facebook()
        {

            return View();
        }


        [AllowAnonymous]
        public ViewResult FreeTips()
        {

            return View();
        }

        [AllowAnonymous]
        public ViewResult Subscribe()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult Faq()
        {
            var model = _db.Questions.ToList();

            return View(model);
        }

        [AllowAnonymous]
        public ViewResult Testimonials()
        {
            var model = _db.Testimonials.ToList();

            return View(model);
        }


        [AllowAnonymous]
        public ViewResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ViewResult Contact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                //_db.Contacts.Add(contact);

                //_db.SaveChanges();

                var SendEmailTo = _configuration.GetValue<string>("ContactEmailAddress");
                var sendGridKey = _configuration.GetValue<string>("SendGridApi");
                //string email, string subject, string htmlContent)
                var message = $"<p>{contact.FirstName} {contact.LastName} have sent you a message on the website. </p> <p>{contact.FirstName} {contact.LastName}</p> <p>{contact.EmailAddress}</p> <p>{contact.Telephone}</p><p>{contact.Message}</>";
                Task.Run(() => Emailer.SendEmail(SendEmailTo, "Email Message from the website.", message, sendGridKey).Wait());

                return View("MessageReceived", contact);
            }

            return View();
        }


        [Authorize]
        public async Task<ActionResult> ElitePackageTips()
        {

            var blockedByAdmin = await BlockedByAdmin("Elite");
            if (blockedByAdmin)
            {
                return View("NotAuthorized");
            }

            List<string> payPalPlanIds = _db.Products.Where(x => x.Name.Contains("Elite")).Select(x => x.PayPalPlanId).ToList();

            var canView = await UserCanViewTips(payPalPlanIds);

            var hasAdminApproval = await HasAdminApproval("Elite");


            if (canView || hasAdminApproval)
            {
                var model = _db.Tips.FirstOrDefault();

                return View(model);
            }

            return View("NotAuthorized");
        }

        [Authorize]
        public async Task<ActionResult> CombinationPackageTips()
        {
            var blockedByAdmin = await BlockedByAdmin("Combo");
            if (blockedByAdmin)
            {
                return View("NotAuthorized");
            }


            List<string> payPalPlanIds = _db.Products.Where(x => x.Name.Contains("combination")).Select(x => x.PayPalPlanId).ToList();

            var canView = await UserCanViewTips(payPalPlanIds);

            var hasAdminApproval = await HasAdminApproval("Combo");

            if (canView || hasAdminApproval)
            {
                var model = _db.Tips.FirstOrDefault();

                return View(model);
            }

            return View("NotAuthorized");
        }

        [Authorize]
        public async Task<ActionResult> UKPackageTips()
        {
            var blockedByAdmin = await BlockedByAdmin("UK");
            if (blockedByAdmin)
            {
                return View("NotAuthorized");
            }

            List<string> payPalPlanIds = _db.Products.Where(x => x.Name.Contains("UK Racing Only")).Select(x => x.PayPalPlanId).ToList();

            var canView = await UserCanViewTips(payPalPlanIds);

            var hasAdminApproval = await HasAdminApproval("UK");           

            if (canView || hasAdminApproval)
            {
                var model = _db.Tips.FirstOrDefault();

                return View(model);
            }

            return View("NotAuthorized");
        }


        [Authorize]
        public async Task<ActionResult> Welcome()
        {
            var blockedUKByAdmin = await BlockedByAdmin("UK");
            var blockedEliteByAdmin = await BlockedByAdmin("Elite");
            var blockedComboByAdmin = await BlockedByAdmin("Combo");
            if (blockedUKByAdmin || blockedEliteByAdmin || blockedComboByAdmin)
            {
                return View("NotAuthorized");
            }
            
            
            List<string> payPalPlanIds = _db.Products.Select(x => x.PayPalPlanId).ToList();

            var canView = await UserCanViewTips(payPalPlanIds); 

            var hasAdminUKApproval = await HasAdminApproval("UK");            

            var hasAdminEliteApproval = await HasAdminApproval("Elite");            

            var hasAdminComboApproval = await HasAdminApproval("Combo");
            
            if (canView ||

               //has access to uk packages                
               hasAdminUKApproval ||

               //has access to Elite packages
               hasAdminEliteApproval ||

               //has access to Combo packages
               hasAdminComboApproval

               )
            {
                var model = _db.Tips.FirstOrDefault();

                return View(model);
            }

            return View("NotAuthorized");
        }


        public ActionResult ComingSoon()
        {
            return View();
        }

        [Authorize(Roles = "superadmin, admin")]
        [HttpGet]
        public FileContentResult EmailList()
        {
            var csvString = GenerateCSVString();
            var fileName = "AndyTipsterEmails " + DateTime.Now.ToString() + ".csv";
            return File(new System.Text.UTF8Encoding().GetBytes(csvString), "text/csv", fileName);
        }

        private string GenerateCSVString()
        {
            List<EmailListViewModel> users = _db.Users.ToList().Select(x =>
          new EmailListViewModel()
          {

              FirstName = x.FirstName,
              LastName = x.LastName,
              Email = x.Email
          }).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append("FirstName");
            sb.Append(",");
            sb.Append("LastName");
            sb.Append(",");
            sb.Append("Email");
            sb.AppendLine();
            foreach (var user in users)
            {
                sb.Append(user.FirstName != null ? user.FirstName : "Not given");
                sb.Append(",");
                sb.Append(user.LastName != null ? user.LastName : "Not given");
                sb.Append(",");
                sb.Append(user.Email);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private async Task<bool> UserCanViewTips(List<string> payPalPlanIds)
        {
            var canView = false;

            //get current user
            var currentUser = await _userManager.GetUserAsync(User);

            //check if user has Any subscription

            if (currentUser != null)
            {
                if (User.Identity.IsAuthenticated && (User.IsInRole("superadmin") || User.IsInRole("admin")))
                {
                    return true;
                }

                var userSubscriptions = _db.UserSubscriptions.Where(x => x.UserId == currentUser.Id).ToList();

                if (userSubscriptions.Count() > 0)
                {
                    ////check if any of the subscription Paypal Id matches with current package.

                    //var canView = payPalPlanIds.Where(n => userSubscriptions.Select(x => x.PayPalPlanId).Contains(n)).Any();



                    //check if user has any given paypal plans.
                    //Also check if user has any active plans.

                    List<string> userPayPalPlans = payPalPlanIds.Where(n => userSubscriptions.Select(x => x.PayPalPlanId).Contains(n)).ToList();

                    DateTime today = DateTime.Now;
                    foreach (var plan in userPayPalPlans)
                    {
                        if (_db.UserSubscriptions.Where(x => x.UserId == currentUser.Id && x.PayPalPlanId == plan && x.State == "Active"
                                                        || (x.UserId == currentUser.Id && x.PayPalPlanId == plan && x.ExpiryDate >= today)

                        ).Any())
                        {
                            canView = true;
                        }
                    }

                    return canView;
                }

            }

            return false;
        }


        private async Task<bool> HasAdminApproval(string packageName)
        {
            //get current user
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            var now = DateTime.UtcNow.Date;

            //check if user has Any subscription

            if (currentUser != null)
            {

                //Check for Elite package.
                if (packageName.Contains("Elite"))
                {
                    return currentUser.CanSeeElitePackage && currentUser.ManualElitePackageAccessExpiresAt > now;
                }

                //Check for Combo package.
                if (packageName.Contains("Combo"))
                {
                    return currentUser.CanSeeComboPackage && currentUser.ManualComboPackageAccessExpiresAt > now;
                }

                //Check for UK package.
                if (packageName.Contains("UK"))
                {
                    return currentUser.CanSeeUKRacingPackage && currentUser.ManualUKRacingPackageAccessExpiresAt > now;
                }
            }

            return false;
        }


        private async Task<bool> BlockedByAdmin(string packageName)
        {
            //get current user
            ApplicationUser currentUser = await _userManager.GetUserAsync(User);

            var now = DateTime.UtcNow.Date;

            //check if user has Any subscription

            if (currentUser != null)
            {

                //Check for Elite package.
                if (packageName.Contains("Elite"))
                {
                    return currentUser.BlockElitePackage;

                }

                //Check for Combo package.
                if (packageName.Contains("Combo"))
                {
                    return currentUser.BlockComboPackage;
                }

                //Check for UK package.
                if (packageName.Contains("UK"))
                {
                    return currentUser.BlockUKRacingPackage;

                }
            }

            return false;
        }


    }

}
