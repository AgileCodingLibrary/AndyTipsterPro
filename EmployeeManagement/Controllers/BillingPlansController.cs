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

        public async Task<IActionResult> Index()
        {

            var client = _clientFactory.GetClient();

            var request = new PlanListRequest()
                .Status("ACTIVE")
                .PageSize("20");


            var result = await client.Execute(request);
            PlanList list = result.Result<PlanList>();

            var hasAnyExistingPlans = _dbContext.BillingPlans.Any();

            var hasAnyProducts = _dbContext.Products.Any();

            if (!hasAnyExistingPlans && list.Plans.Count() > 0)
            {
                foreach (var plan in list.Plans)
                {

                    //add Paypal plans in the database
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

            };

            return View(list);
        }


        //public async Task<ActionResult> SeedProduct()
        //{
        //    List<Product> products = new List<Product>()
        //{
        //    new Product()
        //    {
        //        Name = "Elite Package - Monthly",
        //        Description = "The combination of UK/IRE & International Racing Tips Daily.",
        //        PayPalPlanId = "P-01H67865SG822584NWXTXUVI",
        //        Price = 2800,
        //        PaymentFrequency = "1 Month"
        //    },
        //    new Product()
        //    {
        //        Name = "Elite Package - 3 Months",
        //        Description = "The combination of UK/IRE & International Racing Tips Daily.",
        //        PayPalPlanId = "P-33Y52169CA6617927WXTXX4Y",
        //        Price = 7000,
        //        PaymentFrequency = "3 Months"
        //    },
        //     new Product()
        //    {
        //        Name = "Combination Package UK/IRE - Monthly",
        //        Description = "The UK package but with Irish Racing Tips included",
        //        PayPalPlanId = "P-4PX56472M3478614BWXTX3DA",
        //        Price = 2400,
        //        PaymentFrequency = "1 Month"
        //    },
        //       new Product()
        //    {
        //        Name = "Combination Package UK/IRE -  3 Months",
        //        Description = "The UK package but with Irish Racing Tips included",
        //        PayPalPlanId = "P-4LL517486K0601123WXTX6JA",
        //        Price = 6000,
        //        PaymentFrequency = "3 Months"
        //    },
        //            new Product()
        //    {
        //        Name = "UK Racing Only Package - Monthly",
        //        Description = "Specialising in UK Racing Tips Daily",
        //        PayPalPlanId = "P-07569959YM054623FWXTYBWA",
        //        Price = 1900,
        //        PaymentFrequency = "1 Month"
        //    },
        //                      new Product()
        //    {
        //        Name = "UK Racing Only Package - 3 Months",
        //        Description = "Specialising in UK Racing Tips Daily",
        //        PayPalPlanId = "P-9LD18441MG843805VWXTYX2Y",
        //        Price = 5000,
        //        PaymentFrequency = "3 Months"
        //    }

        //};

        //    _dbContext.Products.AddRange(products);
        //    await _dbContext.SaveChangesAsync();

        //    return View("Index");
        //}

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