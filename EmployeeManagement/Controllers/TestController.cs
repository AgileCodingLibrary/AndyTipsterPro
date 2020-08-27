using AndyTipsterPro.Models;
using AndyTipsterPro.Entities;
using AndyTipsterPro.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal;
using PayPal.Manager;
using PayPal.v1.BillingPlans;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "superadmin")]
    public class TestController : Controller
    {

        private readonly PayPalHttpClientFactory _clientFactory;
        private readonly AppDbContext _dbContext;

        public TestController(PayPalHttpClientFactory clientFactory, AppDbContext dbContext)
        {
            _clientFactory = clientFactory;
            _dbContext = dbContext;
        }


        public async Task<IActionResult> Index()
        {

            await CreatePayPalPlans();

            var client = _clientFactory.GetClient();

            PlanListRequest request = new PlanListRequest()
            .Status("ACTIVE")
            .PageSize("20");


            var result = await client.Execute(request);
            PlanList list = result.Result<PlanList>();

            if (list == null)
            {
                return RedirectToAction("Index", "Home");
            }

            await CreateBillingPlans(list);
            await CreateProducts();

            return View(list);
        }

        private async Task CreateBillingPlans(PlanList list)
        {
            //clear all billing plans, if any.
            var existingBillingPlans = _dbContext.BillingPlans.ToList();

            var plan = list.Plans.Where(x => x.Name.Contains("Test2")).FirstOrDefault();

            //add Billing plans in the database
            var billingPlan = new BillingPlan();

            billingPlan.PayPalPlanId = plan.Id;
            billingPlan.Type = plan.Type;
            billingPlan.Name = plan.Name;
            billingPlan.Description = plan.Description;
            billingPlan.State = plan.State;

            if (plan.PaymentDefinitions != null)
            {
                billingPlan.PaymentFrequency = plan.PaymentDefinitions.FirstOrDefault().Frequency;
                billingPlan.PaymentInterval = plan.PaymentDefinitions.FirstOrDefault().FrequencyInterval;
            }
            else
            {
                billingPlan.PaymentFrequency = "Not Provided";
                billingPlan.PaymentInterval = "Not Provided";
            }

            billingPlan.ReturnURL = "https://www.andytipsterpro.com/";
            billingPlan.CancelURL = "https://www.andytipsterpro.com/";
            billingPlan.CreateTime = plan.CreateTime;
            billingPlan.UpdateTime = plan.UpdateTime;

            _dbContext.BillingPlans.Add(billingPlan);



            await _dbContext.SaveChangesAsync();

        }

        private async Task CreateProducts()
        {

            //clear all products, if any.
            var existingProducts = _dbContext.Products.ToList();

            var billingPlans = _dbContext.BillingPlans.ToList();

            var TestPlan = billingPlans.Where(x => x.Name.Contains("Test2")).FirstOrDefault();


            List<Product> products = new List<Product>();

            var TestProduct = new Product()
            {
                Name = TestPlan.Name,
                Description = TestPlan.Description,
                PayPalPlanId = TestPlan.PayPalPlanId,
                Price = 1,
                PaymentFrequency = "3 Days"
            };
            products.Add(TestProduct);


            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

        }


        /// <summary>
        /// Create the default billing plans for this website
        /// </summary>
        private async Task CreatePayPalPlans()
        {

            var client = _clientFactory.GetClient();

            foreach (var plan in BillingPlanSeedTest.PayPalPlans("https://www.andytipster.com/Subscription/Return", "https://www.andytipster.com/Subscription/Cancel"))

            //foreach (var plan in BillingPlanSeed.PayPalPlans("https://localhost:44376/Subscription/Return", "https://localhost:44376/Subscription/Cancel"))

            {
                // Create Plan
                var request = new PlanCreateRequest().RequestBody(plan);
                BraintreeHttp.HttpResponse result = await client.Execute(request);
                var obj = result.Result<Plan>();

                // Activate Plan
                var activateRequest = new PlanUpdateRequest<Plan>(obj.Id)
                    .RequestBody(GetActivatePlanBody());

                await client.Execute(activateRequest);

            }

        }

        private static List<JsonPatch<Plan>> GetActivatePlanBody()
        {
            return new List<JsonPatch<Plan>>()
            {
                new JsonPatch<Plan>()
                {
                    Op = "replace",
                    Path = "/",
                    Value = new Plan()
                    {
                        State = "ACTIVE"
                    }
                }
            };
        }

        private APIContext GetApiContext()
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential("", "").GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }
    }
}
