using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "superadmin, admin")]
    public class TestimonialController : Controller
    {
        public TestimonialController(AppDbContext context)
        {
            this.db = context;
        }
        private readonly AppDbContext db;

      
        public ActionResult Index()
        {
            var model = db.Testimonials.ToList();

            return View(model);
        }

       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Testimonial test = db.Testimonials.Find(id);

            if (test == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(test);
        }

        
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Testimonial tests)
        {
            if (ModelState.IsValid)
            {
                db.Testimonials.Add(tests);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tests);
        }

       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Testimonial tests = db.Testimonials.Find(id);

            if (tests == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(tests);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Testimonial tests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tests).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tests);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Testimonial tests = db.Testimonials.Find(id);

            if (tests == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(tests);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Testimonial tests = db.Testimonials.Find(id);

            db.Testimonials.Remove(tests);

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
