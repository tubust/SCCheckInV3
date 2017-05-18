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
    public class MemberController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        // GET: Member
        public ActionResult Index()
        {
            return View(db.OKSwingMemberLists.ToList());
        }

        // GET: Member/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OKSwingMemberList oKSwingMemberList = db.OKSwingMemberLists.Find(id);
            if (oKSwingMemberList == null)
            {
                return HttpNotFound();
            }
            return View(oKSwingMemberList);
        }

        // GET: Member/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Member/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MemberID,FirstName,MiddleName,LastName,Address,City,State,Zip,HomePhone,WorkPhone,FaxNumber,EmailAddress,Status,DateJoined,Type,LastPaid,Dues,InputDate,MemberStatus,Organization,DOB,Anniversary,DataEaseMemberNumber,BirthMonth,BirthDay,Charter,NewMemberDate,DateLastUpdated,LastUpdatedBy,Dateadded,AddedBy,Comments,CellPhone,InvalidAddress,IsDeleted,ClassID,isPromoted,currentMonth")] OKSwingMemberList oKSwingMemberList)
        {
            if (ModelState.IsValid)
            {
                db.OKSwingMemberLists.Add(oKSwingMemberList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(oKSwingMemberList);
        }

        // GET: Member/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OKSwingMemberList oKSwingMemberList = db.OKSwingMemberLists.Find(id);
            if (oKSwingMemberList == null)
            {
                return HttpNotFound();
            }
            return View(oKSwingMemberList);
        }

        // POST: Member/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberID,FirstName,MiddleName,LastName,Address,City,State,Zip,HomePhone,WorkPhone,FaxNumber,EmailAddress,Status,DateJoined,Type,LastPaid,Dues,InputDate,MemberStatus,Organization,DOB,Anniversary,DataEaseMemberNumber,BirthMonth,BirthDay,Charter,NewMemberDate,DateLastUpdated,LastUpdatedBy,Dateadded,AddedBy,Comments,CellPhone,InvalidAddress,IsDeleted,ClassID,isPromoted,currentMonth")] OKSwingMemberList oKSwingMemberList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oKSwingMemberList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(oKSwingMemberList);
        }

        // GET: Member/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OKSwingMemberList oKSwingMemberList = db.OKSwingMemberLists.Find(id);
            if (oKSwingMemberList == null)
            {
                return HttpNotFound();
            }
            return View(oKSwingMemberList);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OKSwingMemberList oKSwingMemberList = db.OKSwingMemberLists.Find(id);
            db.OKSwingMemberLists.Remove(oKSwingMemberList);
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
