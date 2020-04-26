using AndyTipsterPro.Models;
using EmployeeManagement.Entities;
using EmployeeManagement.Helpers;
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

            if (!hasAnyExistingPlans && list.Plans.Count() > 0)
            {
                foreach (var plan in list.Plans)
                {
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