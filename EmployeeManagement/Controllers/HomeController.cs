using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;
        private readonly AppDbContext _context;

        public HomeController(IEmployeeRepository employeeRepository,
                              IHostingEnvironment hostingEnvironment,
                              ILogger<HomeController> logger,
                              AppDbContext context)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            _context = context;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult About()
        {
            var model = _context.Abouts.FirstOrDefault();
            if (model != null)
            {
                return View(model);
            }

            return View("Index");
        }



        [AllowAnonymous]
        public ViewResult Subscribe()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult Faq()
        {
            var model = _context.Questions.ToList();

            return View(model);
        }
        [AllowAnonymous]
        public ViewResult Contact()
        {
            return View();
        }


        [Authorize]
        public ActionResult AndyTipsterTips()
        {
            var model = _context.Tips.FirstOrDefault();
            return View(model);
        }


        [Authorize]
        public ActionResult IrishRacingTips()
        {
            var model = _context.Tips.FirstOrDefault();
            return View(model);
        }

        [Authorize]
        public ActionResult UltimatePackTips()
        {
            var model = _context.Tips.FirstOrDefault();
            return View(model);
        }


    }


}
