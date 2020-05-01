using AndyTipsterPro.Entities;
using AndyTipsterPro.Helpers;
using AndyTipsterPro.Models;
using AndyTipsterPro.ViewModels;
using EmployeeManagement.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PayPal.v1.BillingAgreements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndyTipsterPro.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly PayPalHttpClientFactory _clientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(AppDbContext dbContext, PayPalHttpClientFactory clientFactory, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _clientFactory = clientFactory;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var model = new IndexVm()
            {
                BillingPlans = _dbContext.BillingPlans.ToList(),
                Products = _dbContext.Products.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Purchase(string id)
        {
            var model = new PurchaseVm()
            {
                Plan = _dbContext.BillingPlans.FirstOrDefault(x => x.PayPalPlanId == id),
                Product = _dbContext.Products.FirstOrDefault(x => x.PayPalPlanId == id)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(PurchaseVm model)
        {
            var plan = _dbContext.BillingPlans.FirstOrDefault(x => x.PayPalPlanId == model.Product.PayPalPlanId);
            var product = _dbContext.Products.FirstOrDefault(x => x.PayPalPlanId == model.Product.PayPalPlanId);

            //check DUPLICATES
            var currentUser = await _userManager.GetUserAsync(User);
            var userhasAnySubscriptions = _dbContext.UserSubscriptions.Any(x => x.UserId == currentUser.Id);

            if (userhasAnySubscriptions)
            {
                List<UserSubscriptions> subscribedPlans = _dbContext.UserSubscriptions.Where(x => x.UserId == currentUser.Id).ToList();

                bool alreadySusbcribedToThisPlan = subscribedPlans.Any(x => x.PayPalPlanId == product.PayPalPlanId);

                if (alreadySusbcribedToThisPlan)
                {
                    return RedirectToAction("DuplicateSubscriptionFound", product);
                }
            }


            if (ModelState.IsValid && plan != null)
            {
                // Since we take an Initial Payment (instant payment), the start date of the recurring payments will be next month.
                var startDate = DateTime.UtcNow.AddMonths(1);

                var subscription = new Subscription()
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email,
                    StartDate = startDate,
                    PayPalPlanId = plan.PayPalPlanId
                };
                _dbContext.Subscriptions.Add(subscription);


                var userNewSubscriptoin = new UserSubscriptions()
                {
                    PayPalPlanId = plan.PayPalPlanId,
                    Description = plan.Description,
                    User = currentUser,
                    UserId = currentUser.Id
                };

                _dbContext.UserSubscriptions.Add(userNewSubscriptoin);

                _dbContext.SaveChanges();

                var agreement = new Agreement()
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    StartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Plan = new PlanWithId() { Id = Convert.ToString(plan.PayPalPlanId) },
                    //Plan = new global::PayPal.BillingAgreements.Plan()
                    //{
                    //    Id = plan.PayPalPlanId
                    //},
                    Payer = new Payer()
                    {
                        PaymentMethod = "paypal"
                    }
                };


                // Send the agreement to PayPal
                var client = _clientFactory.GetClient();
                var request = new AgreementCreateRequest()
                    .RequestBody(agreement);
                var result = await client.Execute(request);
                Agreement createdAgreement = result.Result<Agreement>();

                // Find the Approval URL to send our user to (also contains the token)
                var approvalUrl =
                    createdAgreement.Links.FirstOrDefault(
                        x => x.Rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));

                var token = QueryHelpers.ParseQuery(approvalUrl?.Href)["token"].First();

                // Save the token so we can match the returned request to our subscription.
                subscription.PayPalAgreementToken = token;


                _dbContext.SaveChanges();

                // Send the user to PayPal to approve the payment
                return Redirect(approvalUrl.Href);

            }

            model.Product = product;
            return View(model);
        }


        public async Task<IActionResult> Return(string token)
        {
            Subscription subscription = _dbContext.Subscriptions.FirstOrDefault(x => x.PayPalAgreementToken == token);

            var client = _clientFactory.GetClient();

            var request = new AgreementExecuteRequest(token);
            request.Body = "{}"; // Bug: Stupid hack workaround for a bug. Lost an hour to this.
            var result = await client.Execute(request);

            var executedAgreement = result.Result<Agreement>();

            // Save the PayPal agreement in our subscription so we can look it up later.
            subscription.PayPalAgreementId = executedAgreement.Id;

            await UpdateUserProfileForSubscriptions(executedAgreement, subscription.PayPalPlanId);

            _dbContext.SaveChanges();

            return RedirectToAction("Thankyou");
        }

        public ActionResult Cancel()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public ActionResult DuplicateSubscriptionFound(Product model)
        {
            return View(model);
        }

        public async Task UpdateUserProfileForSubscriptions(Agreement createdAgreement, string paypalPlanId)
        {

            var user = await _userManager.GetUserAsync(User);

            //add all subscriptions as Payment already has happened!!

            UserSubscriptions subscriptionToUpdate = _dbContext.UserSubscriptions.Where(x => x.PayPalPlanId == paypalPlanId && x.UserId == user.Id).FirstOrDefault();

            if (user != null && subscriptionToUpdate != null)
            {
                subscriptionToUpdate.PayPalAgreementId = createdAgreement.Id;
                subscriptionToUpdate.SubscriptionId = createdAgreement.Id;
                subscriptionToUpdate.State = createdAgreement.State;
                subscriptionToUpdate.Description = createdAgreement.Description;
                subscriptionToUpdate.PayerEmail = createdAgreement.Payer.PayerInfo.Email;
                subscriptionToUpdate.PayerFirstName = createdAgreement.Payer.PayerInfo.FirstName;
                subscriptionToUpdate.PayerLastName = createdAgreement.Payer.PayerInfo.LastName;

                await _dbContext.SaveChangesAsync();
            }

        }
    }
}