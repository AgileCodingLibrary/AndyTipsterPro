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

namespace EmployeeManagement.Controllers
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


        public ActionResult Index()
        {
            var model = new IndexVm()
            {
                BillingPlans = _dbContext.BillingPlans.ToList(),
                Subscriptions = _dbContext.Subscriptions
                    .Where(x => !string.IsNullOrEmpty(x.PayPalAgreementId))
                    .OrderByDescending(x => x.StartDate).Take(50).ToList()
            };

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