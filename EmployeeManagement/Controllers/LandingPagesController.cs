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

        // GET: LandingPages
        public ActionResult Index()
        {
            return View(db.LandingPages.ToList());
        }

        // GET: LandingPages/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LandingPage landingPage = db.LandingPages.Find(id);
        //    if (landingPage == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(landingPage);
        //}

        // GET: LandingPages/Create
        public ActionResult Create()
        {
            return View();
        }

        //// POST: LandingPages/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,LandingPageHtml")] LandingPage landingPage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.LandingPages.Add(landingPage);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(landingPage);
        //}

        // GET: LandingPages/Edit/5

        //public ActionResult Edit(int? id)
        //{
        //    LandingPage landingPage = null;

        //    if (id == null)
        //    {
        //        landingPage = db.LandingPages.FirstOrDefault();
        //    }
        //    else
        //    {
        //        landingPage = db.LandingPages.Find(id);

        //        if (landingPage == null)
        //        {
        //            landingPage = db.LandingPages.FirstOrDefault();
        //        }
        //    }

        //    return View(landingPage);
        //}

        // POST: LandingPages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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

        // GET: LandingPages/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LandingPage landingPage = db.LandingPages.Find(id);
        //    if (landingPage == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(landingPage);
        //}

        // POST: LandingPages/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    LandingPage landingPage = db.LandingPages.Find(id);
        //    db.LandingPages.Remove(landingPage);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
