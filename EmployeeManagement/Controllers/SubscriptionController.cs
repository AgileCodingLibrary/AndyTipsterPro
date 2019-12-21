using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using AndyTipsterPro.Models.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using System;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {

        private Models.AppDbContext _dbContext => HttpContext.GetOwinContext().Get<AppDbContext>();
        public ActionResult Index()
        {
            var model = new IndexVm()
            {
                Plans = Models.Subscription.Plan.Plans
            };

            return View(model);
        }

        public ActionResult Purchase(string id)
        {

            var currentUserId = User.Identity.GetUserId();
            var currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();

            if (currentUser.SubscriptionId != null)
            {
                if (currentUser.SubscriptionState.ToLower() == "active" || currentUser.SubscriptionState.ToLower() == "pending")
                {

                    var existingSubscription = new ExistingSubscriptionVM
                    {
                        //Plan = Models.Subscription.Plan.Plans.FirstOrDefault(x => x.PayPalPlanId == id),
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        Email = currentUser.Email,
                        PlanName = currentUser.SubscriptionDescription,
                        PaypalAgreement = currentUser.PayPalAgreementId
                    };

                    return View("_ExistingSubscription", existingSubscription);

                    ////check which subscription current user has
                    //if (currentUser.SubscriptionDescription == "AndyTipster Monthly Package")
                    //{
                    //    //user has Andy Tipster regular package
                    //    return View("_ExistingSubscription", existingSubscription);

                    //}
                    //else if (currentUser.SubscriptionDescription == "AndyTipster Three Months Package")
                    //{
                    //    //user has Andy Tipster 3 months package
                    //}
                    //else if (currentUser.SubscriptionDescription == "Irish Horse Racing - Monthly Subscription")
                    //{
                    //    //user has Irish Racing regular package
                    //}
                    //else if (currentUser.SubscriptionDescription == "Irish Horse Racing - Three Months Subscription")
                    //{
                    //    //user has Irish 3 months package
                    //}
                    //else if (currentUser.SubscriptionDescription == "Ultimate pack - Monthly Subscription")
                    //{
                    //    //user has Ultimate regular package
                    //}
                    //else if (currentUser.SubscriptionDescription == "Ultimate pack - Three Months Subscription")
                    //{
                    //    //user has Ultimate 3 months package
                    //}
                }
            }
            var model = new PurchaseVm()
            {
                Plan = Models.Subscription.Plan.Plans.FirstOrDefault(x => x.PayPalPlanId == id),
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Email = currentUser.Email
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Purchase(PurchaseVm model)
        {
           
            var plan = Models.Subscription.Plan.Plans.FirstOrDefault(x => x.PayPalPlanId == model.Plan.PayPalPlanId);


            if (ModelState.IsValid)
            {
                // Since we take an Initial Payment (instant payment), the start date of the recurring payments will be next month.
                var startDate = DateTime.UtcNow.AddMonths(1);

                var apiContext = GetApiContext();

                var subscription = new Subscription()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    StartDate = startDate,
                    PayPalPlanId = plan.PayPalPlanId
                };
                _dbContext.Subscriptions.Add(subscription);
                _dbContext.SaveChanges();

                var agreement = new Agreement()
                {
                    name = plan.Name,
                    description = plan.Description,
                    start_date = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    plan = new PayPal.Api.Plan()
                    {
                        id = plan.PayPalPlanId
                    },
                    payer = new Payer()
                    {
                        payment_method = "paypal"
                    }
                };

                // Send the agreement to PayPal
                var createdAgreement = agreement.Create(apiContext);

                // Save the token so we can match the returned request to our subscription.
                subscription.PayPalAgreementToken = createdAgreement.token;
                subscription.PayPalPlanName = createdAgreement.name;

                _dbContext.SaveChanges();

                // Find the Approval URL to send our user to
                var approvalUrl =
                    createdAgreement.links.FirstOrDefault(
                        x => x.rel.Equals("approval_url", StringComparison.OrdinalIgnoreCase));

                // Send the user to PayPal to approve the payment
                return Redirect(approvalUrl.href);
            }

            model.Plan = plan;
            return View(model);
        }

        public ActionResult Return(string token)
        {
            var subscription = _dbContext.Subscriptions.FirstOrDefault(x => x.PayPalAgreementToken == token);

            var apiContext = GetApiContext();

            var agreement = new Agreement()
            {
                token = token
            };

            var executedAgreement = agreement.Execute(apiContext);

            // Save the PayPal agreement in our subscription so we can look it up later.
            subscription.PayPalAgreementId = executedAgreement.id;
            subscription.PayPalPaymentEmail = executedAgreement.payer.payer_info.email;

            //update currently login user to add Paypal Subscription information.

            var currentUserId = User.Identity.GetUserId();
            var currentUser = _dbContext.Users.Where(x => x.Id == currentUserId).FirstOrDefault();
            currentUser.SubscriptionId = executedAgreement.id;
            currentUser.SubscriptionState = executedAgreement.state;
            currentUser.SubscriptionFirstName = executedAgreement.payer.payer_info.first_name;
            currentUser.SubscriptionLastName = executedAgreement.payer.payer_info.last_name;
            currentUser.SubscriptionEmail = executedAgreement.payer.payer_info.email;
            currentUser.SubscriptionDescription = subscription.PayPalPlanName;
            currentUser.PayPalAgreementId = subscription.PayPalAgreementId;

            //user might not have a postal address.
            //currentUser.SubscriptionPostalCode = executedAgreement.payer.payer_info.billing_address.postal_code;

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