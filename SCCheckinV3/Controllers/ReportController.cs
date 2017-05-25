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
    public class ReportController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        public ActionResult Birthdays()
        {
            var birthdayList = db.OKSwingMemberLists.Where(b => (b.BirthMonth == DateTime.Now.Month.ToString() || b.DOB.Value.Month == DateTime.Now.Month) && b.Anniversary > DateTime.Now).OrderBy(o => o.LastName);
            ViewBag.Birthdays = birthdayList;
            return View();
        }

        /* This function uses a chosen month in jQuery to generate a birthday list in Json. */
        [HttpPost]
        public ActionResult Birthdays(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
            {
                beginningDate = DateTime.Now;
            }
            var birthdays = db.OKSwingMemberLists.Where(b => (b.BirthMonth == beginningDate.Month.ToString() || b.DOB.Value.Month == beginningDate.Month) && b.Anniversary > DateTime.Now).OrderBy(o => o.LastName);
            return Json(new { Birthdays = birthdays });
        }

        public ActionResult BlueDancers()
        {
            var blueList = db.OKSwingMemberLists.Where(bl => bl.ClassID == (int)ColorLevel.Blue).OrderBy(o => o.LastName);
            ViewBag.BlueDancers = blueList;
            return View();
        }

        public ActionResult CompleteMemberList()
        {
            var completeList = db.OKSwingMemberLists.ToList().OrderBy(o => o.LastName);
            ViewBag.CompleteMemberList = completeList;
            return View();
        }

        public ActionResult CurrentlyPaidMembers()
        {
            var currentMembers = db.OKSwingMemberLists.Where(an => an.Anniversary > DateTime.Now).OrderBy(o => o.LastName);
            ViewBag.CurrentlyPaidMembers = currentMembers;
            return View();
        }

        public ActionResult DancersCurrentlyInLessons()
        {
            var dancerInLessons = db.CheckIns.Where(d => d.PaidType == (int)PaidType.MonthlyDues || d.PaidType == (int)PaidType.Exempt).Where(p => p.PaidDate.Value.Month == DateTime.Now.Month && p.PaidDate.Value.Year == DateTime.Now.Year)
                .OrderBy(l => l.LastName);
            ViewBag.DancersCurrentlyInLessons = dancerInLessons;
            return View();
        }

        [HttpPost]
        public ActionResult DancersCurrentlyInLessons(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var dancersInLessons = db.CheckIns.Where(d => d.PaidType == (int)PaidType.MonthlyDues || d.PaidType == (int)PaidType.Exempt).Where(p => p.PaidDate.Value.Month == beginningDate.Month && p.PaidDate.Value.Year == beginningDate.Year)
                .OrderBy(o => o.LastName);
            return Json(new { DancersCurrentlyInLessons = dancersInLessons });
        }

        public ActionResult DeletedMembers()
        {
            var deletedMemberList = db.DeletedMembers.ToList();
            ViewBag.DeletedMembers = deletedMemberList;
            return View();
        }

        public ActionResult EmailList()
        {
            var theEmailList = db.OKSwingMemberLists.Where(em => em.EmailAddress != null && em.EmailAddress != string.Empty).OrderBy(o => o.LastName);
            ViewBag.EmailList = theEmailList;
            return View();
        }

        public ActionResult ExpiringMembers()
        {
            DateTime chkAnniversary = DateTime.Now.AddDays(-31);
            var expireList = db.OKSwingMemberLists.Where(ed => ed.Anniversary >= chkAnniversary && ed.Anniversary <= DateTime.Now).OrderBy(o => o.LastName);
            ViewBag.ExpiringMembers = expireList;
            return View();
        }

        public ActionResult FloorRentalOnly()
        {
            var floorView = db.OKSwingMemberLists.Where(f => f.ClassID == (int)ColorLevel.FloorRentalOnly).OrderBy(of => of.LastName);
            ViewBag.FloorRentalOnly = floorView;
            return View();
        }

        public ActionResult GreenDancers()
        {
            var greenDancers = db.OKSwingMemberLists.Where(gr => gr.ClassID == (int)ColorLevel.Green).OrderBy(o => o.LastName);
            ViewBag.GreenDancers = greenDancers;
            return View();
        }

        public ActionResult MembersModifiedInDatabase()
        {
            var membersModified = db.OKSwingMemberLists.Where(mm => mm.DateLastUpdated >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && mm.DateLastUpdated <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            ViewBag.MembersModifiedInDatabase = membersModified;
            return View();
        }

        [HttpPost]
        public ActionResult MembersModifiedInDatabase(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var membersModified = db.OKSwingMemberLists.Where(mm => mm.DateLastUpdated >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && mm.DateLastUpdated <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            return Json(new { MembersModifiedInDatabase = membersModified });
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
            ViewBag.ExpiredAnniversary = expiredAnniversary;
            ViewBag.MissingInAction = missingInAction;
            return View();
        }

        [HttpPost]
        public ActionResult MissingInAction(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
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
            var monthlyDancers = db.CheckIns.Where(m => m.PaidDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && m.PaidDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            ViewBag.MonthlyDancers = monthlyDancers;
            return View();
        }

        [HttpPost]
        public ActionResult MonthlyDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var monthlyDancers = db.CheckIns.Where(m => m.PaidDate >= new DateTime(beginningDate.Year, beginningDate.Month, 1) && m.PaidDate <= new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1));
            return Json(new { MonthlyDancers = monthlyDancers } );
        }

        public ActionResult NewDancers()
        {
            var newDancers = db.OKSwingMemberLists.Where(s => s.NewMemberDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && s.NewMemberDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            ViewBag.NewDancers = newDancers;
            return View();
        }

        [HttpPost]
        public ActionResult NewDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var newDancers = db.OKSwingMemberLists.Where(s => s.NewMemberDate >= new DateTime(beginningDate.Year, beginningDate.Month, 1) && s.NewMemberDate <= new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1));
            return Json(new { NewDancers = newDancers });
        }

        public ActionResult NewMembers()
        {
            var newMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && nm.PaidDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            List<CheckIn> newMemberList = new List<CheckIn>();
            foreach (CheckIn mem in newMemberList)
            {
                if(!isRenewingMember((int)mem.MemberID))
                {
                    newMemberList.Add(mem);
                }
            }
            ViewBag.NewMembers = newMemberList;
            return View();
        }

        [HttpPost]
        public ActionResult NewMembers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var newMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= new DateTime(beginningDate.Year, beginningDate.Month, 1) && nm.PaidDate <= new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1));
            List<CheckIn> newMemberList = new List<CheckIn>();
            foreach (CheckIn mem in newMemberList)
            {
                if (!isRenewingMember((int)mem.MemberID))
                {
                    newMemberList.Add(mem);
                }
            }
            return Json(new { NewMembers = newMemberList });
        }

        public ActionResult NonReturningMembers()
        {
            var nonReturn = db.OKSwingMemberLists.Where(a => a.Anniversary > DateTime.Now && DateTime.Now.Subtract(a.NewMemberDate).Days <= 59);
            List<OKSwingMemberList> nonReturnList = new List<OKSwingMemberList>();
            foreach (OKSwingMemberList nr in nonReturn)
            {
                if(DateTime.Now.Subtract(lastCheckIn(nr.MemberID)).Days > 31)
                {
                    nonReturnList.Add(nr);
                }
            }
            ViewBag.NonReturningMembers = nonReturnList;
            return View();
        }

        public ActionResult PinkDancers()
        {
            var pinkDancers = db.OKSwingMemberLists.Where(pi => pi.ClassID == (int)ColorLevel.Pink).OrderBy(o => o.LastName);
            ViewBag.PinkDancers = pinkDancers;
            return View();
        }

        public ActionResult PurpleDancers()
        {
            var purpleDancers = db.OKSwingMemberLists.Where(pu => pu.ClassID == (int)ColorLevel.Purple).OrderBy(o => o.LastName);
            ViewBag.PurpleDancers = purpleDancers;
            return View();
        }

        public ActionResult RenewingMembers()
        {
            var reNewMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && nm.PaidDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1));
            List<CheckIn> reNewMemberList = new List<CheckIn>();
            foreach (CheckIn mem in reNewMemberList)
            {
                if (isRenewingMember((int)mem.MemberID))
                {
                    reNewMemberList.Add(mem);
                }
            }
            ViewBag.RenewMembers = reNewMemberList;
            return View();
        }

        [HttpPost]
        public ActionResult RenewingMembers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var reNewMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= new DateTime(beginningDate.Year, beginningDate.Month, 1) && nm.PaidDate <= new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1));
            List<CheckIn> reNewMemberList = new List<CheckIn>();
            foreach (CheckIn mem in reNewMemberList)
            {
                if (isRenewingMember((int)mem.MemberID))
                {
                    reNewMemberList.Add(mem);
                }
            }
            return Json(new { ReNewingMembers = reNewMemberList });
        }

        public ActionResult SpecialEvents()
        {
            var specialEvents = db.CheckIns.Where(sp => (sp.PaidType == (int)PaidType.SpecialEvent || sp.PaidDesc.Contains("Special") && sp.PaidDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && sp.PaidDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1)));
            ViewBag.SpecialEvents = specialEvents;
            return View();
        }

        [HttpPost]
        public ActionResult SpecialEvents(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var specialEvents = db.CheckIns.Where(sp => (sp.PaidType == (int)PaidType.SpecialEvent || sp.PaidDesc.Contains("Special")) && sp.PaidDate >= new DateTime(beginningDate.Year, beginningDate.Month, 1) && sp.PaidDate <= new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1));
            return Json(new { SpecialEvents = specialEvents });
        }

        public ActionResult Teachers()
        {
            var teachers = db.OKSwingMemberLists.Where(te => te.ClassID == (int)ColorLevel.Teacher);
            ViewBag.Teachers = teachers;
            return View();
        }

        public ActionResult TodaysDancers()
        {
            var todaysDancers = db.CheckIns.Where(p => p.PaidDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) && p.PaidDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1));
            ViewBag.TodaysDancers = todaysDancers;
            return View();
        }

        [HttpPost]
        public ActionResult TodaysDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var todaysDancers = db.CheckIns.Where(p => p.PaidDate >= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day) && p.PaidDate <= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1));
            return Json(new { TodaysDancers = todaysDancers });
        }

        public ActionResult TodaysPayingDancers()
        {
            var todaysPayers = db.CheckIns.Where(r => r.PaidType != (int)PaidType.CheckIn && r.PaidType != (int)PaidType.Exempt && r.CreateDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) && r.CreateDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1));
            ViewBag.TodaysPayingDancers = todaysPayers;
            return View();
        }

        [HttpPost]
        public ActionResult TodaysPayingDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var todaysPayers = db.CheckIns.Where(r => r.PaidType != (int)PaidType.CheckIn && r.PaidType != (int)PaidType.Exempt && r.CreateDate >= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day) && r.CreateDate <= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1));
            return Json(new { TodaysPayingDancers = todaysPayers });
        }

        public ActionResult TodaysSummary()
        {
            var cashList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CASH")
            && s.CreateDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            && s.CreateDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1));
            var checkList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CHECK")
            && s.CreateDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            && s.CreateDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1));
            var cardList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CREDIT CARD")
            && s.CreateDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            && s.CreateDate <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddSeconds(-1));
            ViewBag.CashList = cashList;
            ViewBag.CheckList = checkList;
            ViewBag.CardList = cardList;
            return View();
        }

        [HttpPost]
        public ActionResult TodaysSummary(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var cashList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CASH")
            && s.CreateDate >= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day)
            && s.CreateDate <= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1));
            var checkList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CHECK")
            && s.CreateDate >= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day)
            && s.CreateDate <= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1));
            var cardList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CREDIT CARD")
            && s.CreateDate >= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day)
            && s.CreateDate <= new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1));
            return Json(new { CashList = cashList, CheckList = checkList, CardList = cardList });
        }

        public ActionResult UnknownDancers()
        {
            var unknownDancers = db.OKSwingMemberLists.Where(un => un.ClassID == (int)ColorLevel.Unknown);
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
            var yearlyDuesList = db.CheckIns.Where(p => p.PaidType == (int)PaidType.YearlyDues && p.PaidDate.Value >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            && p.PaidDate.Value <= new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1).AddSeconds(-1));
            ViewBag.YearlyDues = yearlyDuesList;
            return View();
        }

        [HttpPost]
        public ActionResult YearlyDues(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var yearlyDuesList = db.CheckIns.Where(p => p.PaidType == (int)PaidType.YearlyDues && p.PaidDate.Value >= new DateTime(beginningDate.Year, beginningDate.Month, 1)
            && p.PaidDate.Value <= new DateTime(beginningDate.Year, beginningDate.Month + 1, 1).AddSeconds(-1));
            return Json(new { YearlyDues = yearlyDuesList });
        }

        public ActionResult YellowDancers()
        {
            var yellowList = db.OKSwingMemberLists.Where(y => y.ClassID == (int)ColorLevel.Yellow).OrderBy(o => o.LastName);
            ViewBag.YellowDancers = yellowList;
            return View();
        }

        private bool isRenewingMember(int memberID)
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
         * 23 = Unknown Dancers
         * 24 = Voided Entries
         * 25 = Yearly Dues
         * 26 = Year Over Year Sales
         * 27 = Yellow Dancers
         */
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
                case 23:
                    break;
                case 24:
                    break;
                case 25:
                    break;
                case 26:
                    break;
                case 27:
                    break;
                default:
                    break;
            }
            return View();
        }
    }
}
