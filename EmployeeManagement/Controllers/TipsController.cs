using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AndyTipsterPro.Controllers
{
    [Authorize(Roles = "superadmin, admin")]
    public class TipsController : Controller
    {
        public TipsController(AppDbContext context)
        {
            this.db = context;
        }

        private readonly AppDbContext db;

        [Authorize(Roles = "superadmin")]
        public ActionResult Index()
        {
            var model = db.Tips.ToList();

            return View(model);
        }

        [Authorize(Roles = "superadmin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Tips tips = db.Tips.Find(id);

            if (tips == null)
            {
                return RedirectToAction("Index");
            }

            return View(tips);
        }


        [Authorize(Roles = "superadmin")]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "superadmin")]
        public ActionResult Create(Tips tips)
        {
            if (ModelState.IsValid)
            {
                db.Tips.Add(tips);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(tips);
        }

        public ActionResult Edit(int? id)
        {
            Tips tips = null;

            //super admin can edit more than one record.
            if (User.Identity.IsAuthenticated && User.IsInRole("superadmin"))
            {
                if (id == null)
                {
                    tips = db.Tips.FirstOrDefault();
                }
                else
                {
                    tips = db.Tips.Find(id);

                    if (tips == null)
                    {
                        tips = db.Tips.FirstOrDefault();
                    }
                }
            }
            else // user can only edit one record.
            {
                if (id == null)
                {
                    tips = db.Tips.FirstOrDefault();
                }
                else
                {
                    tips = db.Tips.Find(id);

                    if (tips == null)
                    {
                        tips = db.Tips.FirstOrDefault();
                    }
                }
            }

            if (tips == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(tips);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tips tips)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tips).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index", new { controller = "Home" });
            }
            return View(tips);
        }


        [Authorize(Roles = "superadmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            Tips tips = db.Tips.Find(id);

            if (tips == null)
            {
                return RedirectToAction("Index");
            }
            return View(tips);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "superadmin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Tips tips = db.Tips.Find(id);

            db.Tips.Remove(tips);

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
