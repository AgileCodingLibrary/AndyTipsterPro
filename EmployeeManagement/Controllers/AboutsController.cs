using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "superadmin, admin")]

    public class AboutsController : Controller
    {
        public AboutsController(AppDbContext context)
        {
            this.db = context;
        }

        private readonly AppDbContext db;


        [Authorize(Roles = "superadmin")]
        public ActionResult Index()
        {
            return View(db.Abouts.ToList());

        }

        [Authorize(Roles = "superadmin")]
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            About about = db.Abouts.Find(id);

            if (about == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(about);
        }

        [Authorize(Roles = "superadmin")]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "superadmin")]
        public ActionResult Create(About about)
        {
            if (ModelState.IsValid)
            {
                db.Abouts.Add(about);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(about);
        }


        public ActionResult Edit(int? id)
        {
            About about = null;

            //super admin can edit more than one record.
            if (User.Identity.IsAuthenticated && User.IsInRole("superadmin"))
            {

                if (id == null)
                {
                    about = db.Abouts.FirstOrDefault();
                }
                else
                {
                    about = db.Abouts.Find(id);

                    if (about == null)
                    {
                        about = db.Abouts.FirstOrDefault();
                    }
                }
            }
            else // user can only edit one record.
            {
                about = db.Abouts.FirstOrDefault();

                if (about == null)
                {
                    return RedirectToAction("Index", new { controller = "Home" });
                }
            }

            if (about == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(about);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(About about)
        {
            if (ModelState.IsValid)
            {
                db.Entry(about).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(about);

        }

        [Authorize(Roles = "superadmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            About about = db.Abouts.Find(id);
            if (about == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(about);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "superadmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            About about = db.Abouts.Find(id);
            db.Abouts.Remove(about);
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
