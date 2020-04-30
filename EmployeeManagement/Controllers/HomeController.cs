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

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger,
                              AppDbContext db,
                              IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            _db = db;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var model = _db.LandingPages.FirstOrDefault();

            return View(model);
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
                _db.Contacts.Add(contact);

                _db.SaveChanges();

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
        public ActionResult AndyTipsterTips()
        {
            var model = _db.Tips.FirstOrDefault();
            return View(model);
        }


        [Authorize]
        public ActionResult IrishRacingTips()
        {
            var model = _db.Tips.FirstOrDefault();
            return View(model);
        }

        [Authorize]
        public ActionResult UltimatePackTips()
        {
            var model = _db.Tips.FirstOrDefault();
            return View(model);
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


    }


}
