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
        public ActionResult Birthdays(DateTime startDate)
        {
            DateTime beginningDate;
            if (startDate != null)
            {
                beginningDate = new DateTime(startDate.Year, startDate.Month, 1);
            }
            else
            {
                // Sets to Todays Month in case of failure.
                beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            var birthdays = db.OKSwingMemberLists.Where(b => b.BirthMonth == beginningDate.Month.ToString());
            return Json(new { Birthdays = birthdays });
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
            ViewBag.CompleteMemberList = completeList;
            return View();
        }

        public ActionResult CurrentlyPaidMembers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CurrentlyPaidMembers(DateTime startDate)
        {
            return View();
        }

        public ActionResult DancersCurrentlyInLessons()
        {
            var dancerInLessons = db.CheckIns.Where(d => d.PaidType == 1 || d.PaidType == 7).Where(p => p.PaidDate.Value.Month == DateTime.Now.Month && p.PaidDate.Value.Year == DateTime.Now.Year)
                .OrderBy(l => l.LastName);
            ViewBag.DancersInLessons = dancerInLessons;
            return View();
        }

        [HttpPost]
        public ActionResult DancersCurrentlyInLessons(DateTime startDate)
        {
            DateTime beginningDate;
            if(startDate != null)
            {
                beginningDate = startDate;
            }
            else
            {
                beginningDate = DateTime.Now;
            }
            var dancersInLessons = db.CheckIns.Where(d => d.PaidType == 1 || d.PaidType == 7).Where(p => p.PaidDate.Value.Month == beginningDate.Month && p.PaidDate.Value.Year == beginningDate.Year);
            return Json(new { DancersInLessons = dancersInLessons });
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

        [HttpPost]
        public ActionResult ExpiringMembers(DateTime startDate)
        {
            return View();
        }

        public ActionResult GreenDancers()
        {
            var greenDancers = db.OKSwingMemberLists.Where(gr => gr.ClassID == 5);
            ViewBag.GreenDancers = greenDancers;
            return View();
        }

        public ActionResult MembersModifiedInDatabase()
        {
            return View();
        }

        /* Missing in Action means a dancer whos last recorded check in is over 60 days ago.*/
        public ActionResult MissingInAction()
        {
            var memberList = db.OKSwingMemberLists.ToList();
            List<OKSwingMemberList> missingInAction = new List<OKSwingMemberList>();
            List<OKSwingMemberList> expiredAnniversary = new List<OKSwingMemberList>();
            DateTime sixtyDays = DateTime.Now.AddDays(-60);
            foreach(OKSwingMemberList mem in memberList)
            {
                if(mem.Anniversary < DateTime.Now)
                {
                    expiredAnniversary.Add(mem);
                }
                if(sixtyDays > lastCheckIn(mem.MemberID))
                {
                    missingInAction.Add(mem);
                }
            }
            ViewBag.ExpiringAnniversary = expiredAnniversary;
            ViewBag.MissingInAction = missingInAction;
            return View();
        }

        [HttpPost]
        public ActionResult MissingInAction(DateTime startDate)
        {
            DateTime beginningDate;
            if(startDate != null)
            {
                beginningDate = startDate;
            }
            else
            {
                beginningDate = DateTime.Now;
            }
            var memberList = db.OKSwingMemberLists.ToList();
            List<OKSwingMemberList> missingInAction = new List<OKSwingMemberList>();
            List<OKSwingMemberList> expiredAnniversary = new List<OKSwingMemberList>();
            DateTime sixtyDays = beginningDate.AddDays(-60);
            foreach (OKSwingMemberList mem in memberList)
            {
                if (mem.Anniversary < DateTime.Now)
                {
                    expiredAnniversary.Add(mem);
                }
                if (sixtyDays > lastCheckIn(mem.MemberID))
                {
                    missingInAction.Add(mem);
                }
            }
            return Json(new { ExpiredAnniversary = expiredAnniversary, MissingInAction = missingInAction } );
        }

        public ActionResult MonthlyDancers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MonthlyDancers(DateTime startDate)
        {
            return View();
        }

        public ActionResult NewDancers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewDancers(DateTime startDate)
        {
            return View();
        }

        public ActionResult NewMembers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewMembers(DateTime startDate)
        {
            return View();
        }

        public ActionResult NonReturningMembers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NonReturningMembers(DateTime startDate)
        {
            return View();
        }

        public ActionResult PinkDancers()
        {
            var pinkDancers = db.OKSwingMemberLists.Where(pi => pi.ClassID == 1);
            ViewBag.PinkDancers = pinkDancers;
            return View();
        }

        public ActionResult PurpleDancers()
        {
            var purpleDancers = db.OKSwingMemberLists.Where(pu => pu.ClassID == 2);
            ViewBag.PurpleDancers = purpleDancers;
            return View();
        }

        public ActionResult RenewingMembers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RenewingMembers(DateTime startDate)
        {
            return View();
        }

        public ActionResult SpecialEvents()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SpecialEvents(DateTime startDate)
        {
            return View();
        }

        public ActionResult Teachers()
        {
            var teachers = db.OKSwingMemberLists.Where(te => te.ClassID == 7);
            ViewBag.Teachers = teachers;
            return View();
        }

        public ActionResult TodaysDancers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TodaysDancers(DateTime startDate)
        {
            return View();
        }

        public ActionResult TodaysPayingDancers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TodaysPayingDancers(DateTime startDate)
        {
            return View();
        }

        public ActionResult TodaysSummary()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TodaysSummary(DateTime startDate)
        {
            return View();
        }

        public ActionResult UnknownDancers()
        {
            var unknownDancers = db.OKSwingMemberLists.Where(un => un.ClassID == 0);
            ViewBag.UnknownDancers = unknownDancers;
            return View();
        }

        public ActionResult VoidedEntries()
        {
            var voidEntry = db.VoidedEntries.ToList();
            ViewBag.VoidEntry = voidEntry;
            return View();
        }

        public ActionResult YearlyDues()
        {
            var yearlyDuesList = db.CheckIns.Where(p => p.PaidType == 2 && p.PaidDate.Value >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            && p.PaidDate.Value <= new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddSeconds(-1));
            ViewBag.YearlyDues = yearlyDuesList;
            return View();
        }

        [HttpPost]
        public ActionResult YearlyDues(DateTime startDate)
        {
            return View();
        }

        /* this method takes a number code to make a report into an Excel spreadsheet.
         * 0 = Birthdays
         * 1 = Blue Dancers
         * 2 = Complete List
         * 3 = Currently Paid Members
         * 4 = Dancers in Lessons
         * 5 = Deleted Members
         * 6 = Email List
         * 7 = Expiring Members
         * 8 = Green Dancers
         * 9 = Modified Members
         * 10 = Missing In Action
         * 11 = Monthly Dancers
         * 12 = New Dancers
         * 13 = New Members
         * 14 = Non Returning Members
         * 15 = Pink Dancers
         * 16 = Purple Dancers
         * 17 = Renewing Members
         * 18 = Special Events
         * 19 = Teacher List
         * 20 = Todays Dancers
         * 21 = Todays Paying Dancers
         * 22 = Todays Summary
         */

        private bool isRenewingMember(int? memberID)
        {
            int memID = (int)memberID;
            int yearlyCount = db.CheckIns.Count(ye => ye.DanceType == 2 && ye.MemberID == memID);
            return (yearlyCount > 1);
        }

        private DateTime lastCheckIn(int memberID)
        {
            var checkInDates = db.CheckIns.Where(ch => ch.MemberID == memberID).OrderByDescending(c => c.CreateDate);
            foreach(CheckIn chk in checkInDates)
            {
                return chk.CreateDate;
            }
            return DateTime.Now;
        }

        public ActionResult convertToExcel(int whichReport)
        {
            switch (whichReport)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 18:
                    break;
                case 19:
                    break;
                case 20:
                    break;
                case 21:
                    break;
                case 22:
                    break;
                default:
                    break;
            }
            return View();
        }
    }
}
