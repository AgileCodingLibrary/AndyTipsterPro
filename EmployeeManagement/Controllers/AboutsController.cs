using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AboutsController : Controller
    {
        public AboutsController(AppDbContext context)
        {
            this.db = context;
        }
        
        private readonly AppDbContext db;


        public ActionResult Index()
        {
            return View(db.Abouts.ToList());
        }


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


        public ActionResult Create()
        {
            return View();
        }

        //POST: Abouts/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        
            return View(about);
        }

        // POST: Abouts/Edit/5      
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


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            About about = db.Abouts.Find(id);
            if (about == null)
            {
                return HttpNotFound();
            }
            return View(about);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
