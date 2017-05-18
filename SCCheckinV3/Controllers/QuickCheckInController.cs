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
    public class QuickCheckInController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        // GET: QuickCheckIn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuickCheckIn/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CheckInID,MemberID,FullName,FirstName,LastName,PaidType,PaidAmount,PaidDate,PaidDesc,DanceType,CreateDate,ReferredBy")] CheckIn checkIn)
        {
            if (ModelState.IsValid)
            {
                db.CheckIns.Add(checkIn);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(checkIn);
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
