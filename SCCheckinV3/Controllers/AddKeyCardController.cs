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
    public class AddKeyCardController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        // GET: AddKeyCard
        public ActionResult Index()
        {
            return View(db.KeyCardTables.ToList());
        }

        // GET: AddKeyCard/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeyCardTable keyCardTable = db.KeyCardTables.Find(id);
            if (keyCardTable == null)
            {
                return HttpNotFound();
            }
            return View(keyCardTable);
        }

        // GET: AddKeyCard/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AddKeyCard/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PrimaryKey,KeyCardNumber,LastName,FirstName,MemberID,IsAdmin,MadeAdminBy")] KeyCardTable keyCardTable)
        {
            if (ModelState.IsValid)
            {
                db.KeyCardTables.Add(keyCardTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keyCardTable);
        }

        // GET: AddKeyCard/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeyCardTable keyCardTable = db.KeyCardTables.Find(id);
            if (keyCardTable == null)
            {
                return HttpNotFound();
            }
            return View(keyCardTable);
        }

        // POST: AddKeyCard/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PrimaryKey,KeyCardNumber,LastName,FirstName,MemberID,IsAdmin,MadeAdminBy")] KeyCardTable keyCardTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(keyCardTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(keyCardTable);
        }

        // GET: AddKeyCard/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeyCardTable keyCardTable = db.KeyCardTables.Find(id);
            if (keyCardTable == null)
            {
                return HttpNotFound();
            }
            return View(keyCardTable);
        }

        // POST: AddKeyCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KeyCardTable keyCardTable = db.KeyCardTables.Find(id);
            db.KeyCardTables.Remove(keyCardTable);
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
