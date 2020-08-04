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


namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "superadmin")]
    public class BillingPlansController : Controller
    {
        private readonly PayPalHttpClientFactory _clientFactory;
        private readonly AppDbContext _dbContext;

        public BillingPlansController(PayPalHttpClientFactory clientFactory, AppDbContext dbContext)
        {
            _clientFactory = clientFactory;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
          
            return RedirectToAction("Index", "Home");
        }

        //public async Task<IActionResult> Index()
        //{

        //    var hasAnyPayPalPlans = _dbContext.BillingPlans.Any();
        //    if (!hasAnyPayPalPlans)
        //    {
        //        await CreatePayPalPlans();
        //    }

        //    return RedirectToAction("Index", "Home");

        //    var client = _clientFactory.GetClient();

        //    PlanListRequest request = new PlanListRequest()
        //    .Status("ACTIVE")
        //    .PageSize("20");


        //    var result = await client.Execute(request);
        //    PlanList list = result.Result<PlanList>();

        //    if (list == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    await CreateBillingPlans(list);
        //    await CreateProducts();

        //    return View(list);
        //}


        //public async Task<IActionResult> Index() //Adding plans.
        //{


        //    await CreatePayPalPlans();


        //    //return RedirectToAction("Index", "Home");

        //    var client = _clientFactory.GetClient();

        //    PlanListRequest request = new PlanListRequest()
        //    .Status("ACTIVE")
        //    .PageSize("20");


        //    var result = await client.Execute(request);
        //    PlanList list = result.Result<PlanList>();

        //    if (list == null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    await AddBillingPlans(list);
        //    await AddProducts();

        //    return View(list);
        //}

        private async Task CreateBillingPlans(PlanList list)
        {
            //clear all billing plans, if any.
            var existingBillingPlans = _dbContext.BillingPlans.ToList();
            if (existingBillingPlans.Count() > 0)
            {
                _dbContext.BillingPlans.RemoveRange(existingBillingPlans);
                _dbContext.SaveChanges();

            }

            foreach (var plan in list.Plans)
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

            }

            await _dbContext.SaveChangesAsync();

        }

        private async Task CreateProducts()
        {

            //clear all products, if any.
            var existingProducts = _dbContext.Products.ToList();
            if (existingProducts.Count() > 0)
            {
                _dbContext.Products.RemoveRange(existingProducts);
                _dbContext.SaveChanges();

            }

            var billingPlans = _dbContext.BillingPlans.ToList();

            var EliteMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - Monthly").FirstOrDefault();
            var EliteQuarterBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - 3 Months").FirstOrDefault();
            var CombinationMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE - Monthly").FirstOrDefault();
            var CombinationQuarterBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE -  3 Months").FirstOrDefault();
            var UKRacingMonthlyBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - Monthly").FirstOrDefault();
            var UKRacingQuarterBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - 3 Months").FirstOrDefault();


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

        }

        private async Task AddBillingPlans(PlanList list)
        {

            foreach (var plan in list.Plans)
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

            }

            await _dbContext.SaveChangesAsync();

        }

        private async Task AddProducts() // new products in august 2020.
        {


            var billingPlans = _dbContext.BillingPlans.ToList();

            var EliteMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - Monthly (1001)").FirstOrDefault();
            var EliteQuarterBillingPlan = billingPlans.Where(x => x.Name == "Elite Package - 3 Months (1001)").FirstOrDefault();
            var CombinationMonthlyBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE - Monthly (1001)").FirstOrDefault();
            var CombinationQuarterBillingPlan = billingPlans.Where(x => x.Name == "Combination Package UK/IRE -  3 Months (1001)").FirstOrDefault();
            var UKRacingMonthlyBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - Monthly (1001)").FirstOrDefault();
            var UKRacingQuarterBillingPlan = billingPlans.Where(x => x.Name == "UK Racing Only Package - 3 Months (1001)").FirstOrDefault();


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

        }

        /// <summary>
        /// Create the default billing plans for this website
        /// </summary>
        private async Task CreatePayPalPlans()
        {

            var client = _clientFactory.GetClient();

            foreach (var plan in BillingPlanSeed.PayPalPlans("https://www.andytipster.com/Subscription/Return", "https://www.andytipster.com/Subscription/Cancel"))

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




                // Add to database record
                //var dbPlan = _dbContext.BillingPlans.FirstOrDefault(x =>
                //    x.Name == obj.Name);

                //if (dbPlan != null && string.IsNullOrEmpty(dbPlan.PayPalPlanId))
                //{
                //    dbPlan.PayPalPlanId = obj.Id;
                //    await _dbContext.SaveChangesAsync();
                //}


            }

            //// Create plans
            //var justBrowsingPlanRequest = new PlanCreateRequest().RequestBody(justBrowsingPlan);
            //var justBrowsingPlanResult = await client.Execute(justBrowsingPlanRequest);
            //var justBrowsingPlanObject = justBrowsingPlanResult.Result<Plan>();

            //var letsDoThisPlanRequest = new PlanCreateRequest().RequestBody(letsDoThisPlan);
            //var letsDoThisPlanResult = await client.Execute(letsDoThisPlanRequest);
            //var letsDoThisPlanObject = letsDoThisPlanResult.Result<Plan>();

            //var beardIncludedPlanRequest = new PlanCreateRequest().RequestBody(beardIncludedPlan);
            //var beardIncludedPlanResult = await client.Execute(beardIncludedPlanRequest);
            //var beardIncludedPlanObject = beardIncludedPlanResult.Result<Plan>();

            //var hookItToMyVeinsPlanRequest = new PlanCreateRequest().RequestBody(hookItToMyVeinsPlan);
            //var hookItToMyVeinsPlanResult = await client.Execute(hookItToMyVeinsPlanRequest);
            //var hookItToMyVeinsPlanObject = hookItToMyVeinsPlanResult.Result<Plan>();

            //// Activate plans
            //var activateJustBrowsingPlanRequest = new PlanUpdateRequest<Plan>(justBrowsingPlanObject.Id)
            //    .RequestBody(GetActivatePlanBody());
            //await client.Execute(activateJustBrowsingPlanRequest);

            //var activateletsDoThisPlanRequest = new PlanUpdateRequest<Plan>(letsDoThisPlanObject.Id)
            //    .RequestBody(GetActivatePlanBody());
            //await client.Execute(activateletsDoThisPlanRequest);

            //var activateBeardIncludedPlanRequest = new PlanUpdateRequest<Plan>(beardIncludedPlanObject.Id)
            //    .RequestBody(GetActivatePlanBody());
            //await client.Execute(activateBeardIncludedPlanRequest);

            //var activateHookItToMyVeinsPlanRequest = new PlanUpdateRequest<Plan>(hookItToMyVeinsPlanObject.Id)
            //    .RequestBody(GetActivatePlanBody());
            //await client.Execute(activateHookItToMyVeinsPlanRequest);
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