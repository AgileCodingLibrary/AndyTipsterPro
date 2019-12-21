using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AndyTipsterPro.Entities;
using AndyTipsterPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndyTipsterPro.Controllers
{
    public class TipsController : Controller
    {
        public TipsController(AppDbContext context)
        {
            this.db = context;
        }

        private readonly AppDbContext db;

        public ActionResult Index()
        {
            //return View(db.Tips.ToList());
            return RedirectToAction("Index");
        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    Tips tips = db.Tips.Find(id);
        //    if (tips == null)
        //    {
        //        tips = db.Tips.FirstOrDefault();               
        //    }

        //    return View(tips);
        //}


        //public ActionResult Create()
        //{
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,AndyTipsterTips,IrishHorseTips,UltimateTips")] Tips tips)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Tips.Add(tips);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(tips);
        //}

        public ActionResult Edit(int? id)
        {
            Tips tips = null;

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
 
            return View(tips);
        }

        // POST: Tips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tips tips)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tips).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { controller = "Home"});
            }
            return View(tips);
        }

        // GET: Tips/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Tips tips = db.Tips.Find(id);
        //    if (tips == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tips);
        //}

        // POST: Tips/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Tips tips = db.Tips.Find(id);
        //    db.Tips.Remove(tips);
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
