using AndyTipsterPro.Models;
using EmployeeManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            // Check for database records and add new plans if missing
            var hasMissingPayPalPlans = _dbContext.BillingPlans.Any(x => string.IsNullOrEmpty(x.PayPalPlanId));
            if (hasMissingPayPalPlans)
            {
                await SeedBillingPlans();
            }

            var client = _clientFactory.GetClient();

            var request = new PlanListRequest()
                .Status("ACTIVE")
                .PageSize("20");

            var result = await client.Execute(request);
            PlanList list = result.Result<PlanList>();

            return View(list);
        }



        /// <summary>
        /// Create the default billing plans for this example website
        /// </summary>
        private async Task SeedBillingPlans()
        {
            var client = _clientFactory.GetClient();

            foreach (var plan in BillingPlanSeed.PayPalPlans(
                Url.Action("Return", "Subscription"),
                Url.Action("Cancel", "Subscription")))
            {
                // Create Plan
                var request = new PlanCreateRequest().RequestBody(plan);
                var result = await client.Execute(request);
                var obj = result.Result<Plan>();

                // Activate Plan
                var activateRequest = new PlanUpdateRequest<Plan>(obj.Id)
                    .RequestBody(GetActivatePlanBody());
                await client.Execute(activateRequest);

                // Add to database record
                var dbPlan = _dbContext.BillingPlans.FirstOrDefault(x =>
                    x.Name == obj.Name);

                if (dbPlan != null && string.IsNullOrEmpty(dbPlan.PayPalPlanId))
                {
                    dbPlan.PayPalPlanId = obj.Id;
                    await _dbContext.SaveChangesAsync();
                }
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
    }
}