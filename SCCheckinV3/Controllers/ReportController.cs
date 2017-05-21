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
    [Authorize]
    public class ReportController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();
        
        public ActionResult Birthdays()
        {
            var birthdayList = db.OKSwingMemberLists.Where(b => b.BirthMonth == DateTime.Now.Month.ToString());
            ViewBag.Birthdays = birthdayList;
            return View();
        }

        /* This function uses a chosen month in jQuery to generate a birthday list in Json. */
        [HttpPost]
        public ActionResult Birthdays(DateTime? startDate)
        {
            if(startDate != null)
            {

            }
            return View();
        }

        public ActionResult BlueDancers()
        {
            var blueList = db.OKSwingMemberLists.Where(bl => bl.ClassID == 6);
            ViewBag.BlueList = blueList;
            return View();
        }

        public ActionResult CompleteMemberList()
        {
            var completeList = db.OKSwingMemberLists.ToList();
            return View();
        }

        public ActionResult CurrentlyPaidMembers()
        {
            return View();
        }

        public ActionResult DancersCurrentlyInLessons()
        {
            return View();
        }

        public ActionResult DeletedMembers()
        {
            var deletedMemberList = db.DeletedMembers.ToList();
            ViewBag.DeletedMemberList = deletedMemberList;
            return View();
        }

        public ActionResult EmailList()
        {
            var theEmailList = db.OKSwingMemberLists.Where(em => em.EmailAddress != null && em.EmailAddress != string.Empty);
            ViewBag.EmailList = theEmailList;
            return View();
        }

        public ActionResult ExpiringMembers()
        {
            return View();
        }

        public ActionResult GreenDancers()
        {
            return View();
        }

        public ActionResult MembersModifiedInDatabase()
        {
            return View();
        }

        public ActionResult MissingInAction()
        {
            return View();
        }

        public ActionResult MonthlyDancers()
        {
            return View();
        }

        public ActionResult NewDancers()
        {
            return View();
        }

        public ActionResult NewMembers()
        {
            return View();
        }

        public ActionResult NonReturningMembers()
        {
            return View();
        }

        public ActionResult PinkDancers()
        {
            return View();
        }

        public ActionResult PurpleDancers()
        {
            return View();
        }

        public ActionResult RenewingMembers()
        {
            return View();
        }

        public ActionResult SpecialEvents()
        {
            return View();
        }

        public ActionResult Teachers()
        {
            return View();
        }

        public ActionResult TodaysDancers()
        {
            return View();
        }

        public ActionResult TodaysPayingDancers()
        {
            return View();
        }

        public ActionResult TodaysSummary()
        {
            return View();
        }

        public ActionResult UnknownDancers()
        {
            return View();
        }

        public ActionResult VoidedEntries()
        {
            return View();
        }

        public ActionResult YearlyDues()
        {
            return View();
        }

        public ActionResult convertToExcel(int whichReport)
        {
            switch (whichReport)
            {
                default:
                    break;
            }
            return View();
        }
    }
}
