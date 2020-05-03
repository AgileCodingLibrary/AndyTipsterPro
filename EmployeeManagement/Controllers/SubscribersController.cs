using AndyTipsterPro.Helpers;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using PayPal.v1.BillingPlans;
using PayPal.v1.BillingAgreements;
using PayPal;
using AndyTipsterPro.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using AndyTipsterPro.Entities;
using System;

namespace AndyTipsterPro.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly PayPalHttpClientFactory _clientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscribersController(AppDbContext dbContext, PayPalHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _clientFactory = clientFactory;
            _userManager = userManager;
        }


        public async Task<ActionResult> Index()
        {
            List<Entities.UserSubscriptions> subscriptions = _dbContext.UserSubscriptions.Where(x => x.PayPalPlanId != null && x.PayPalAgreementId != null).ToList();
            var users = new List<ApplicationUser>();

            var model = new List<Subscriber>();

            foreach (var sub in subscriptions)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(sub.UserId);
                users.Add(user);

                var subscriber = new Subscriber
                {
                    ApplicationUserFirstName = user.FirstName,
                    ApplicationUserLastName = user.LastName,
                    ApplicationUserEmail = user.Email,

                    PayerFirstName = sub.PayerFirstName,
                    PayerLastName = sub.PayerLastName,
                    PayerEmail = sub.PayerEmail,
                    StartDate = sub.StartDate,
                    PayPalPlanId = sub.PayPalPlanId,
                    PayPalAgreementId = sub.PayPalAgreementId,
                    PayPalPlanDescription = sub.Description
                };

                model.Add(subscriber);
            }

            return View(model);
        }



        public async Task<IActionResult> Details(string id)
        {
            var client = _clientFactory.GetClient();

            var request = new AgreementGetRequest(id);
            var result = await client.Execute(request);
            var agreement = result.Result<Agreement>();

            return View(agreement);
        }

        public async Task<IActionResult> Suspend(string id)
        {
            var client = _clientFactory.GetClient();

            var request = new AgreementSuspendRequest(id).RequestBody(new AgreementStateDescriptor()
            {
                Note = "Suspended"
            });
            await client.Execute(request);

            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Reactivate(string id)
        {
            var client = _clientFactory.GetClient();

            var request = new AgreementReActivateRequest(id).RequestBody(new AgreementStateDescriptor()
            {
                Note = "Reactivated"
            });
            await client.Execute(request);

            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Cancel(string id)
        {
            var client = _clientFactory.GetClient();

            var request = new AgreementCancelRequest(id).RequestBody(new AgreementStateDescriptor()
            {
                Note = "Cancelled"
            });
            await client.Execute(request);

            return RedirectToAction("Details", new { id = id });
        }
    }
}