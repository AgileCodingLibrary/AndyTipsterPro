using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using System;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "Masteradmin")]
    public class SubscribersController : Controller
    {
        private Models.AppDbContext _dbContext => HttpContext.GetOwinContext().Get<Models.AppDbContext>();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "CreatedDate" : "";
           

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var subscriptions = _dbContext.Subscriptions.ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                subscriptions = subscriptions.Where(s => s.LastName.Contains(searchString) ||
                                                         s.FirstName.Contains(searchString) ||
                                                         s.Email.Contains(searchString) ||
                                                          s.PayPalPaymentEmail.Contains(searchString) ||
                                                           s.PayPalPlanName.Contains(searchString)
                                                         ).ToList();
            }
            switch (sortOrder)
            {
                case "Email":
                    subscriptions = subscriptions.OrderByDescending(s => s.Email).ToList();
                    break;
                case "PayPalEmail":
                    subscriptions = subscriptions.OrderByDescending(s => s.PayPalPaymentEmail).ToList();
                    break;
                case "FirstName":
                    subscriptions = subscriptions.OrderByDescending(s => s.FirstName).ToList();
                    break;
                case "LastName":
                    subscriptions = subscriptions.OrderByDescending(s => s.LastName).ToList();
                    break;
                case "CreatedDate":
                    subscriptions = subscriptions.OrderByDescending(s => s.StartDate).ToList();
                    break;
                case "Plan":
                    subscriptions = subscriptions.OrderByDescending(s => s.PayPalPlanName).ToList();
                    break;
                default:  // Name ascending 
                    subscriptions = subscriptions.OrderByDescending(s => s.StartDate).ToList();
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(subscriptions.ToPagedList(pageNumber, pageSize));


            //var subscriptions = _dbContext.Subscriptions.OrderByDescending(x => x.StartDate).Take(50).ToList();
            //return View(subscriptions);
        }

        public ActionResult Details(string id)
        {
            var apiContext = GetApiContext();

            var agreement = Agreement.Get(apiContext, id);

            return View(agreement);
        }

        public ActionResult Suspend(string id)
        {
            var apiContext = GetApiContext();

            Agreement.Suspend(apiContext, id, new AgreementStateDescriptor()
            {
                note = "Suspended"
            });

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Reactivate(string id)
        {
            var apiContext = GetApiContext();

            Agreement.ReActivate(apiContext, id, new AgreementStateDescriptor()
            {
                note = "Reactivated"
            });

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Cancel(string id)
        {
            var apiContext = GetApiContext();

            Agreement.Cancel(apiContext, id, new AgreementStateDescriptor()
            {
                note = "Cancelled"
            });

            return RedirectToAction("Details", new { id = id });
        }



        private APIContext GetApiContext()
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }
    }
}