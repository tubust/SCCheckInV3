using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCCheckinV3;

namespace SCCheckinV3.Controllers
{
    public class FeeController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        // GET: Fee
        public ActionResult Index()
        {
            return View(db.tblFees.ToList());
        }

        // GET: Fee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFee tblFee = db.tblFees.Find(id);
            if (tblFee == null)
            {
                return HttpNotFound();
            }
            return View(tblFee);
        }

        // GET: Fee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblFee tblFee = db.tblFees.Find(id);
            if (tblFee == null)
            {
                return HttpNotFound();
            }
            return View(tblFee);
        }

        // POST: Fee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FeeID,FeeCode,FeeAmount")] tblFee tblFee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblFee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblFee);
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
