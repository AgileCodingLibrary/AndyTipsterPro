using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using AndyTipsterPro.Helpers;
using AndyTipsterPro.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PayPal.v1.BillingAgreements;
using System;
using System.Linq;
using System.Threading.Tasks;
using PayPal.v1.BillingPlans;
using Microsoft.AspNetCore.Identity;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "superadmin")]
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
                Product = _dbContext.Products.FirstOrDefault(x=>x.PayPalPlanId == id)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Purchase(PurchaseVm model)
        {
            var plan = _dbContext.BillingPlans.FirstOrDefault(x => x.PayPalPlanId == model.Product.PayPalPlanId);
            var product = _dbContext.Products.FirstOrDefault(x => x.PayPalPlanId == model.Product.PayPalPlanId);

            var currentUser = await _userManager.GetUserAsync(User);

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
                _dbContext.SaveChanges();

                var agreement = new Agreement()
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    StartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Plan = new PlanWithId() { Id = Convert.ToString(plan.PayPalPlanId)},
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
                var createdAgreement = result.Result<Agreement>();

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
            var subscription = _dbContext.Subscriptions.FirstOrDefault(x => x.PayPalAgreementToken == token);

            var client = _clientFactory.GetClient();

            var request = new AgreementExecuteRequest(token);
            request.Body = "{}"; // Bug: Stupid hack workaround for a bug. Lost an hour to this.
            var result = await client.Execute(request);

            var executedAgreement = result.Result<Agreement>();

            // Save the PayPal agreement in our subscription so we can look it up later.
            subscription.PayPalAgreementId = executedAgreement.Id;
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
    }
}