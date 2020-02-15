using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    public class LandingPagesController : Controller
    {
        public LandingPagesController(AppDbContext context)
        {
            this.db = context;
        }

        private readonly AppDbContext db;


        public ActionResult Index()
        {
            var model = db.LandingPages.ToList();

            return View(model);
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var model = db.LandingPages.Find(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LandingPage landingPage)
        {

            if (ModelState.IsValid)
            {
                db.LandingPages.Add(landingPage);

                db.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(landingPage);
        }



        public ActionResult Edit(int? id)
        {

            LandingPage landingPage = null;

            //super admin can edit more than one record.
            if (User.Identity.IsAuthenticated && User.IsInRole("superadmin"))
            {

                if (id == null)
                {
                    landingPage = db.LandingPages.FirstOrDefault();
                }
                else
                {
                    landingPage = db.LandingPages.Find(id);

                    if (landingPage == null)
                    {
                        landingPage = db.LandingPages.FirstOrDefault();
                    }
                }
            }
            else // user can only edit one record.
            {
                landingPage = db.LandingPages.FirstOrDefault();

                if (landingPage == null)
                {
                    return RedirectToAction("Index", new { controller = "Home" });
                }
            }


            return View(landingPage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LandingPage landingPage)
        {

            if (ModelState.IsValid)
            {
                db.Entry(landingPage).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { controller = "Home" });

            }

            return View(landingPage);
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var model = db.LandingPages.Find(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LandingPage landingPage = db.LandingPages.Find(id);

            db.LandingPages.Remove(landingPage);

            db.SaveChanges();

            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
