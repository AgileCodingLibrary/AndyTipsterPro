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
using Microsoft.AspNetCore.Mvc.RazorPages;

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

       
        public IActionResult Index()
        {
            return View();
        }


        //public async Task<IActionResult> Index()
        
        //{


        //    //await CreatePayPalPlans();

        //    var client = _clientFactory.GetClient();

        //    //PlanListRequest request = new PlanListRequest()
        //    //.Status("ACTIVE")
        //    //.PageSize("20");


        //    //var result = await client.Execute(request);
        //    //PlanList list = result.Result<PlanList>();

        //    var first20 = new PlanListRequest().Page("0").PageSize("20").Status("ACTIVE");  

        //    var first20Result = await client.Execute(first20);

        //    //create a list for first 20 in the list.
        //    PlanList list = first20Result.Result<PlanList>();

        //    var next20 = new PlanListRequest().Page("1").PageSize("20").Status("ACTIVE");

        //    var next20Result = await client.Execute(next20);

        //    var next20List = next20Result.Result<PlanList>();

        //    foreach (var plan in next20List.Plans)
        //    {

        //        if (plan.Name.Contains("092020"))
        //        {
        //            list.Plans.Add(plan);
        //        }
                
        //    }

            
        //    if (list == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    //await CreateBillingPlans(list);
        //    //await CreateProducts();

        //    return View(list);
        //}

        private async Task CreateBillingPlans(PlanList list)
        {
            //clear all billing plans, if any.
            var existingBillingPlans = _dbContext.BillingPlans.ToList();

            var plans = list.Plans.Where(x => x.Name.Contains("092020")).ToList();

            foreach (var plan in plans)
            {
                if (plan != null)
                {
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
            }

                     

        }

        private async Task CreateProducts()
        {

            var billingPlans = _dbContext.BillingPlans.ToList();

            var EliteMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - Monthly (092020)").FirstOrDefault();
            var EliteQuarterBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - 3 Months (092020)").FirstOrDefault();
            var CombinationMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE - Monthly (092020)").FirstOrDefault();
            var CombinationQuarterBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE -  3 Months (092020)").FirstOrDefault();
            var UKRacingMonthlyBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - Monthly (092020)").FirstOrDefault();
            var UKRacingQuarterBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - 3 Months (092020)").FirstOrDefault();

            List<Product> products = new List<Product>();

            var EliteMonthlyProduct = new Product()
            {
                Name = EliteMonthlyBillingPlan.Name,
                Description = EliteMonthlyBillingPlan.Description,
                PayPalPlanId = EliteMonthlyBillingPlan.PayPalPlanId,
                Price = 2800,
                PaymentFrequency = "1 Month"
            };
            products.Add(EliteMonthlyProduct);

            var EliteQuarterProduct = new Product()
            {
                Name = EliteQuarterBillingPlan.Name,
                Description = EliteQuarterBillingPlan.Description,
                PayPalPlanId = EliteQuarterBillingPlan.PayPalPlanId,
                Price = 7000,
                PaymentFrequency = "3 Months"
            };
            products.Add(EliteQuarterProduct);

            var CombinationMonthlyProduct = new Product()
            {
                Name = CombinationMonthlyBillingPlan.Name,
                Description = CombinationMonthlyBillingPlan.Description,
                PayPalPlanId = CombinationMonthlyBillingPlan.PayPalPlanId,
                Price = 2400,
                PaymentFrequency = "1 Month"
            };
            products.Add(CombinationMonthlyProduct);

            var CombinationQuarterProduct = new Product()
            {
                Name = CombinationQuarterBillingPlan.Name,
                Description = CombinationQuarterBillingPlan.Description,
                PayPalPlanId = CombinationQuarterBillingPlan.PayPalPlanId,
                Price = 6000,
                PaymentFrequency = "3 Months"
            };
            products.Add(CombinationQuarterProduct);

            var UKRacingMonthlyProduct = new Product()
            {
                Name = UKRacingMonthlyBillingPlan.Name,
                Description = UKRacingMonthlyBillingPlan.Description,
                PayPalPlanId = UKRacingMonthlyBillingPlan.PayPalPlanId,
                Price = 1900,
                PaymentFrequency = "1 Month"
            };
            products.Add(UKRacingMonthlyProduct);


            var UKRacingQuarterProduct = new Product()
            {
                Name = UKRacingQuarterBillingPlan.Name,
                Description = UKRacingQuarterBillingPlan.Description,
                PayPalPlanId = UKRacingQuarterBillingPlan.PayPalPlanId,
                Price = 5000,
                PaymentFrequency = "3 Months"
            };
            products.Add(UKRacingQuarterProduct);

            _dbContext.Products.AddRange(products);
            await _dbContext.SaveChangesAsync();

            //var TestPlan = billingPlans.Where(x => x.Name.Contains("092020")).FirstOrDefault();

            //if (TestPlan != null)
            //{
            //    List<Product> products = new List<Product>();

            //    var TestProduct = new Product()
            //    {
            //        Name = TestPlan.Name,
            //        Description = TestPlan.Description,
            //        PayPalPlanId = TestPlan.PayPalPlanId,
            //        Price = 1,
            //        PaymentFrequency = "3 Days"
            //    };
            //    products.Add(TestProduct);


            //    _dbContext.Products.AddRange(products);
            //    await _dbContext.SaveChangesAsync();
            //}


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
