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
    public class QuestionsController : Controller
    {
        public QuestionsController(AppDbContext context)
        {
            this.db = context;
        }
        private readonly AppDbContext db;

      
        public ActionResult Index()
        {
            var model = db.Questions.ToList();

            return View(model);
        }

       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Questions questions = db.Questions.Find(id);

            if (questions == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(questions);
        }

        
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Questions questions)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(questions);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(questions);
        }

       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Questions questions = db.Questions.Find(id);

            if (questions == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(questions);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Questions questions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questions).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(questions);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Questions questions = db.Questions.Find(id);

            if (questions == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(questions);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Questions questions = db.Questions.Find(id);

            db.Questions.Remove(questions);

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
