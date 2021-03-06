﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SCCheckinV3;
using System.IO;
using System.Text;

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

        public ActionResult ExpiredMembers()
        {
            var expiredMembers = db.OKSwingMemberLists.Where(an => an.Anniversary <= DateTime.Now && an.EmailAddress != null && an.EmailAddress != string.Empty).OrderByDescending(a => a.Anniversary);
            ViewBag.ExpiredMembers = expiredMembers;
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
            DateTime beginTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1);
            var membersModified = db.OKSwingMemberLists.Where(mm => mm.DateLastUpdated >= beginTime && mm.DateLastUpdated <= endTime);
            ViewBag.MembersModifiedInDatabase = membersModified;
            return View();
        }

        [HttpPost]
        public ActionResult MembersModifiedInDatabase(DateTime startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            if (DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = new DateTime(beginningDate.Year,beginningDate.Month,1);
            else
                beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            endDate = new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1);
            var membersModified = db.OKSwingMemberLists.Where(mm => mm.DateLastUpdated >= beginningDate && mm.DateLastUpdated <= endDate);
            return Json(new { MembersModifiedInDatabase = membersModified });
        }

        /* Missing in Action means a dancer whos last recorded check in is over 60 days ago.*/
        public ActionResult MissingInAction()
        {
            var memberList = db.OKSwingMemberLists.Where(er => er.EmailAddress != null && er.EmailAddress != string.Empty);
            List<OKSwingMemberList> missingInAction = new List<OKSwingMemberList>();
            DateTime sixtyDays = DateTime.Now.AddDays(-60);
            foreach(OKSwingMemberList mem in memberList)
            {
                DateTime lastCheck = LastCheckIn(mem.MemberID);
                if (sixtyDays > lastCheck)
                {
                    mem.Dateadded = lastCheck.ToShortDateString();
                    missingInAction.Add(mem);
                }
            }
            ViewBag.MissingInAction = missingInAction.OrderBy(o => o.LastName);
            return View();
        }

        [HttpPost]
        public ActionResult MissingInAction(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var memberList = db.OKSwingMemberLists.Where(er => er.EmailAddress != null && er.EmailAddress != string.Empty).ToList();
            List<OKSwingMemberList> missingInAction = new List<OKSwingMemberList>();
            DateTime sixtyDays = beginningDate.AddDays(-60);
            foreach (OKSwingMemberList mem in memberList)
            {
                DateTime lastCheck = LastCheckIn(mem.MemberID);
                if (sixtyDays > lastCheck)
                {
                    mem.Dateadded = lastCheck.ToShortDateString();
                    missingInAction.Add(mem);
                }
            }
            return Json(new { MissingInAction = missingInAction.OrderBy(o => o.LastName) } );
        }

        public ActionResult MonthlyDancers()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1);
            var monthlyDancers = db.CheckIns.Where(m => m.PaidDate >= beginningDate && m.PaidDate <= endDate).OrderBy(o => o.LastName).ThenBy(f => f.FirstName);
            ViewBag.MonthlyDancers = monthlyDancers;
            return View();
        }

        [HttpPost]
        public ActionResult MonthlyDancers(DateTime startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            endDate = new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1);
            var monthlyDancers = db.CheckIns.Where(m => m.PaidDate >= beginningDate && m.PaidDate <= endDate).OrderBy(o => o.LastName).ThenBy(f => f.FirstName);
            return Json(new { MonthlyDancers = monthlyDancers } );
        }

        public ActionResult NewDancers()
        {
            DateTime beginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddSeconds(-1);
            var newDancers = db.OKSwingMemberLists.Where(s => s.NewMemberDate >= beginDate && s.NewMemberDate <= endDate).OrderBy(o => o.LastName);
            ViewBag.NewDancers = newDancers;
            return View();
        }

        [HttpPost]
        public ActionResult NewDancers(DateTime startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            endDate = new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1);
            var newDancers = db.OKSwingMemberLists.Where(s => s.NewMemberDate >= beginningDate && s.NewMemberDate <= endDate).OrderBy(o => o.LastName);
            return Json(new { NewDancers = newDancers });
        }

        public ActionResult NewMembers()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = beginningDate.AddMonths(1).AddSeconds(-1);
            var newMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= beginningDate && nm.PaidDate <= endDate).OrderBy(o => o.LastName);
            List<OKSwingMemberList> newMemberList = new List<OKSwingMemberList>();
            foreach (CheckIn mem in newMembers)
            {
                if(!IsRenewingMember((int)mem.MemberID))
                {
                    List<OKSwingMemberList> theNewMember = db.OKSwingMemberLists.Where(m => m.MemberID == mem.MemberID).ToList();
                    newMemberList.Add(theNewMember[0]);
                }
            }
            ViewBag.NewMembers = newMemberList.OrderBy(l => l.LastName);
            return View();
        }

        [HttpPost]
        public ActionResult NewMembers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            DateTime endDate = beginningDate.AddMonths(1).AddSeconds(-1);
            var newMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= beginningDate && nm.PaidDate <= endDate);
            List<OKSwingMemberList> newMemberList = new List<OKSwingMemberList>();
            foreach (CheckIn mem in newMembers)
            {
                if (!IsRenewingMember((int)mem.MemberID))
                {
                    List<OKSwingMemberList> newMemberItem = db.OKSwingMemberLists.Where(m => m.MemberID == mem.MemberID).ToList();
                    newMemberList.Add(newMemberItem[0]);
                }
            }
            return Json(new { NewMembers = newMemberList.OrderBy(l => l.LastName) });
        }

        public ActionResult NonReturningMembers()
        {
            DateTime beginningDate = DateTime.Now;
            var nonReturn = db.OKSwingMemberLists.Where(a => a.Anniversary > beginningDate).OrderBy(l => l.LastName);
            List<OKSwingMemberList> nonReturnList = new List<OKSwingMemberList>();
            foreach (OKSwingMemberList nr in nonReturn)
            {
                if(DateTime.Now.Subtract(LastCheckIn(nr.MemberID)).Days > 31)
                {
                    if (nr.Address != string.Empty || nr.HomePhone != string.Empty)
                    {
                        nonReturnList.Add(nr);
                    }
                }
            }
            ViewBag.NonReturningMembers = nonReturnList;
            return View();
        }

        [HttpPost]
        public ActionResult NonReturningMembers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            var nonReturn = db.OKSwingMemberLists.Where(a => a.Anniversary > beginningDate).OrderBy(l => l.LastName).ThenBy(f => f.FirstName);
            List<OKSwingMemberList> nonReturnList = new List<OKSwingMemberList>();
            foreach (OKSwingMemberList nr in nonReturn)
            {
                if (DateTime.Now.Subtract(LastCheckIn(nr.MemberID)).Days > 31)
                {
                    if (nr.Address != string.Empty || nr.HomePhone != string.Empty)
                    {
                        nonReturnList.Add(nr);
                    }
                }
            }
            return Json(new { NonReturningMembers = nonReturnList });
        }

        public ActionResult PinkDancers()
        {
            var pinkDancers = db.OKSwingMemberLists.Where(pi => pi.ClassID == (int)ColorLevel.Pink).OrderBy(o => o.LastName).ThenBy(f => f.FirstName);
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
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = beginningDate.AddMonths(1).AddSeconds(-1);
            var reNewMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= beginningDate && nm.PaidDate <= endDate);
            List<OKSwingMemberList> reNewMemberList = new List<OKSwingMemberList>();
            foreach (CheckIn mem in reNewMembers)
            {
                if (IsRenewingMember((int)mem.MemberID))
                {
                    List<OKSwingMemberList> theMember = db.OKSwingMemberLists.Where(me => me.MemberID == mem.MemberID).ToList();
                    reNewMemberList.Add(theMember[0]);
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
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            DateTime endDate = beginningDate.AddMonths(1).AddSeconds(-1);
            var reNewMembers = db.CheckIns.Where(nm => nm.PaidType == (int)PaidType.YearlyDues && nm.PaidDate >= beginningDate && nm.PaidDate <= endDate);
            List<OKSwingMemberList> reNewMemberList = new List<OKSwingMemberList>();
            foreach (CheckIn mem in reNewMembers)
            {
                if (IsRenewingMember((int)mem.MemberID))
                {
                    List<OKSwingMemberList> theMember = db.OKSwingMemberLists.Where(me => me.MemberID == mem.MemberID).ToList();
                    reNewMemberList.Add(theMember[0]);
                }
            }
            return Json(new { RenewMembers = reNewMemberList });
        }

        public ActionResult SpecialEvents()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1);
            var specialEvents = db.CheckIns.Where(sp => (sp.PaidType == (int)PaidType.SpecialEvent || sp.PaidDesc.Contains("Special")) && sp.PaidDate >= beginningDate && sp.PaidDate <= endDate);
            ViewBag.SpecialEvents = specialEvents;
            return View();
        }

        [HttpPost]
        public ActionResult SpecialEvents(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, 1).AddMonths(1).AddSeconds(-1);
            var specialEvents = db.CheckIns.Where(sp => (sp.PaidType == (int)PaidType.SpecialEvent || sp.PaidDesc.Contains("Special")) && sp.PaidDate >= beginningDate && sp.PaidDate <= endDate);
            return Json(new { SpecialEvents = specialEvents });
        }

        public ActionResult Teachers()
        {
            var teachers = db.OKSwingMemberLists.Where(te => te.ClassID == (int)ColorLevel.Teacher);
            ViewBag.Teachers = teachers;
            return View();
        }

        public ActionResult ThreeYearSales()
        {
            DateTime beginningDate = DateTime.Now.AddYears(-3);
            DateTime endDate = DateTime.Now;
            var yearToYear = db.CheckIns.Where(p => p.PaidType != (int)PaidType.Exempt && p.PaidType != (int)PaidType.CheckIn && p.CreateDate >= beginningDate && p.CreateDate <= endDate).OrderBy(a => a.PaidType);
            ViewBag.ThreeYearSales = yearToYear;
            return View();
        }

        [HttpPost]
        public ActionResult ThreeYearSales(DateTime startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            if (!DateTime.TryParse(startDate.ToString(), out endDate))
            {
                endDate = DateTime.Now;
            }
            beginningDate = endDate.AddYears(-3);
            var yearToYear = db.CheckIns.Where(p => p.PaidType != (int)PaidType.Exempt && p.PaidType != (int)PaidType.CheckIn && p.CreateDate >= beginningDate && p.CreateDate <= endDate).OrderBy(a => a.PaidType);
            JsonResult ThreeYearSales = Json(yearToYear);
            ThreeYearSales.MaxJsonLength = Int32.MaxValue;
            return ThreeYearSales;
        }

        public ActionResult TodaysDancers()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime endDate = beginningDate.AddDays(1).AddSeconds(-1);
            var todaysDancers = db.CheckIns.Where(p => p.PaidDate >= beginningDate && p.PaidDate <= endDate).OrderBy(o => o.LastName).ThenBy(f => f.FirstName);
            ViewBag.TodaysDancers = todaysDancers;
            return View();
        }

        [HttpPost]
        public ActionResult TodaysDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day);
            DateTime endDate = beginningDate.AddDays(1).AddSeconds(-1);
            var todaysDancers = db.CheckIns.Where(p => p.PaidDate >= beginningDate && p.PaidDate <= endDate).OrderBy(o => o.LastName).ThenBy(f => f.FirstName);
            return Json(new { TodaysDancers = todaysDancers });
        }

        public ActionResult TodaysPayingDancers()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime endDate = beginningDate.AddDays(1).AddSeconds(-1);
            var todaysPayers = db.CheckIns.Where(r => r.PaidType != (int)PaidType.CheckIn && r.PaidType != (int)PaidType.Exempt && r.CreateDate >= beginningDate && r.CreateDate <= endDate).OrderBy(o => o.PaidDesc).ThenBy(l => l.LastName).ThenBy(f => f.FirstName);
            ViewBag.TodaysPayingDancers = todaysPayers;
            return View();
        }

        [HttpPost]
        public ActionResult TodaysPayingDancers(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day);
            DateTime endDate = beginningDate.AddDays(1).AddSeconds(-1);
            var todaysPayers = db.CheckIns.Where(r => r.PaidType != (int)PaidType.CheckIn && r.PaidType != (int)PaidType.Exempt && r.CreateDate >= beginningDate && r.CreateDate <= endDate).OrderBy(o => o.PaidDesc).ThenBy(l => l.LastName).ThenBy(f => f.FirstName);
            return Json(new { TodaysPayingDancers = todaysPayers });
        }

        public ActionResult TodaysSummary()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1);
            var cashList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CASH")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
            var checkList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CHECK")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
            var cardList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CREDIT CARD")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
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
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddDays(1).AddSeconds(-1);
            var cashList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CASH")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
            var checkList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CHECK")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
            var cardList = db.CheckIns.Where(s => s.PaidAmount > 0 && s.PaidDesc.EndsWith("PAID BY CREDIT CARD")
                && s.CreateDate >= beginningDate && s.CreateDate <= endDate);
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
            var voidEntry = db.VoidedEntries.OrderByDescending(c => c.CreateDate);
            ViewBag.VoidedEntries = voidEntry;
            return View();
        }

        public ActionResult YearlyDues()
        {
            DateTime beginningDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddMonths(1).AddSeconds(-1);
            var yearlyDuesList = db.CheckIns.Where(p => p.PaidType == (int)PaidType.YearlyDues && p.PaidDate.Value >= beginningDate
            && p.PaidDate.Value <= endDate);
            ViewBag.YearlyDues = yearlyDuesList;
            return View();
        }

        [HttpPost]
        public ActionResult YearlyDues(DateTime startDate)
        {
            DateTime beginningDate;
            if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                beginningDate = DateTime.Now;
            beginningDate = new DateTime(beginningDate.Year, beginningDate.Month, 1);
            DateTime endDate = new DateTime(beginningDate.Year, beginningDate.Month, beginningDate.Day).AddMonths(1).AddSeconds(-1);
            var yearlyDuesList = db.CheckIns.Where(p => p.PaidType == (int)PaidType.YearlyDues && p.PaidDate.Value >= beginningDate
            && p.PaidDate.Value <= endDate);
            return Json(new { YearlyDues = yearlyDuesList });
        }

        public ActionResult YearOverYearSales()
        {
            DateTime beginningDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now;
            var yearToYear = db.CheckIns.Where(p => p.PaidType != (int)PaidType.Exempt && p.PaidType != (int)PaidType.CheckIn && p.CreateDate >= beginningDate && p.CreateDate <= endDate).OrderBy(a => a.PaidType);
            ViewBag.YearOverYearSales = yearToYear;
            return View();
        }

        [HttpPost]
        public ActionResult YearOverYearSales(DateTime startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            if(!DateTime.TryParse(startDate.ToString(), out endDate))
            {
                endDate = DateTime.Now;
            }
            beginningDate = endDate.AddYears(-1);
            var yearToYear = db.CheckIns.Where(p => p.PaidType != (int)PaidType.Exempt && p.PaidType != (int)PaidType.CheckIn && p.CreateDate >= beginningDate && p.CreateDate <= endDate).OrderBy(a => a.PaidType);
            return Json(new { YearOverYearSales = yearToYear });
        }

        public ActionResult YellowDancers()
        {
            var yellowList = db.OKSwingMemberLists.Where(y => y.ClassID == (int)ColorLevel.Yellow).OrderBy(o => o.LastName);
            ViewBag.YellowDancers = yellowList;
            return View();
        }

        private bool IsRenewingMember(int memberID)
        {
            int memID = (int)memberID;
            int yearlyCount = db.CheckIns.Count(ye => ye.PaidType == (int)PaidType.YearlyDues && ye.MemberID == memID);
            return (yearlyCount > 1);
        }

        public DateTime LastCheckIn(int memberID)
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
         * 7 = Expired Members
         * 8 = Expiring Members
         * 9 = Green Dancers
         * 10 = Modified Members
         * 11 = Missing In Action
         * 12 = Monthly Dancers
         * 13 = New Dancers
         * 14 = New Members
         * 15 = Non Returning Members
         * 16 = Pink Dancers
         * 17 = Purple Dancers
         * 18 = Renewing Members
         * 19 = Special Events
         * 20 = Teacher List
         * 21 = Three Year Sales
         * 22 = Todays Dancers
         * 23 = Todays Paying Dancers
         * 24 = Todays Summary
         * 25 = Unknown Dancers
         * 26 = Voided Entries
         * 27 = Yearly Dues
         * 28 = Year Over Year Sales
         * 29 = Yellow Dancers
         * 30 = Floor Rental Only
         */
        public FileContentResult ConvertToExcel(int whichReport, DateTime? startDate)
        {
            DateTime beginningDate;
            DateTime endDate;
            StringBuilder theFileContents = new StringBuilder();
            switch (whichReport)
            {
                case 0:
                    if (!DateTime.TryParse(startDate.ToString(), out beginningDate))
                    {
                        beginningDate = DateTime.Now;
                    }
                    var birthdays = db.OKSwingMemberLists.Where(b => (b.BirthMonth == beginningDate.Month.ToString() || b.DOB.Value.Month == beginningDate.Month) && b.Anniversary > DateTime.Now).OrderBy(o => o.LastName);
                    try
                    {
                        theFileContents.AppendLine("LastName,FirstName,BirthDate,Address,City,State,ZipCode");
                        foreach (OKSwingMemberList mem in birthdays)
                        {
                            if(mem.BirthMonth != null || mem.BirthDay != null)
                            {
                                theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + mem.BirthMonth + "/" + mem.BirthDay + "," + mem.Address + "," + mem.City + "," + mem.State + "," + mem.Zip);
                            }
                            else
                            {
                                theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + mem.DOB.Value.Month + "/" + mem.DOB.Value.Day + "," + mem.Address.Replace(",", "") + "," + mem.City.Replace(",", "") + "," + mem.State.Replace(",", "") + "," + mem.Zip.Replace(",", ""));
                            }
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "MemberBirthdaysFor" + beginningDate.ToString("MMMM") + ".csv");
                case 1:
                    var blueDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Blue);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach(OKSwingMemberList mem in blueDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",BLUE");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "BlueDancers.csv");
                case 2:
                    var completeList = db.OKSwingMemberLists.ToList().OrderBy(o => o.LastName);
                    try
                    {
                        theFileContents.AppendLine("LastName,FirstName,Address,City,State,Zip,Phone,Email,Date Joined");
                        foreach(OKSwingMemberList mem in completeList)
                        {
                            string filteredAddress = mem.Address == null ? string.Empty : mem.Address.Replace(",", "");
                            string filteredCity = mem.City == null ? string.Empty : mem.City.Replace(",", "");
                            string filteredState = mem.State == null ? string.Empty : mem.State.Replace(",", "");
                            string filteredPhone = mem.HomePhone == null ? string.Empty : mem.HomePhone.Replace(",", "");
                            theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + filteredAddress + "," + filteredCity + "," + filteredState + "," + mem.Zip + "," + filteredPhone + "," + mem.EmailAddress + "," + mem.DateJoined);
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "CompleteMemberList.csv");
                case 3:
                    var currentMembers = db.OKSwingMemberLists.Where(an => an.Anniversary > DateTime.Now).OrderBy(o => o.LastName);
                    try
                    {
                        theFileContents.AppendLine("LastName,FirstName,Address,City,State,Zip,Phone,Email,Anniversary");
                        foreach (OKSwingMemberList mem in currentMembers)
                        {
                            string filteredAddress = mem.Address == null ? string.Empty : mem.Address.Replace(",", "");
                            string filteredCity = mem.City == null ? string.Empty : mem.City.Replace(",", "");
                            string filteredState = mem.State == null ? string.Empty : mem.State.Replace(",", "");
                            string filteredPhone = mem.HomePhone == null ? string.Empty : mem.HomePhone.Replace(",", "");
                            theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + filteredAddress + "," + filteredCity + "," + filteredState + "," + mem.Zip + "," + filteredPhone + "," + mem.EmailAddress + "," + mem.Anniversary);
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "CurrentlyPaidMembers.csv");
                case 4:
                    var dancerInLessons = db.CheckIns.Where(d => d.PaidType == (int)PaidType.MonthlyDues || d.PaidType == (int)PaidType.Exempt).Where(p => p.PaidDate.Value.Month == DateTime.Now.Month && p.PaidDate.Value.Year == DateTime.Now.Year)
                        .OrderBy(l => l.LastName);
                    try
                    {
                        theFileContents.AppendLine("Last Name, First Name");
                        foreach (CheckIn mem in dancerInLessons)
                        {
                            theFileContents.AppendLine(mem.LastName + "," + mem.FirstName);
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "DancersInLessons.csv");
                case 5:
                    var deletedMemberList = db.DeletedMembers.ToList();
                    try
                    {
                        theFileContents.AppendLine("LastName,FirstName,Address,City,State,Zip,Phone,Email,Date Joined,Authorized By 1,Authorized By 2");
                        foreach (DeletedMember mem in deletedMemberList)
                        {
                            string filteredAddress = mem.Address == null ? string.Empty : mem.Address.Replace(",", "");
                            string filteredCity = mem.City == null ? string.Empty : mem.City.Replace(",", "");
                            string filteredState = mem.State == null ? string.Empty : mem.State.Replace(",", "");
                            string filteredPhone = mem.HomePhone == null ? string.Empty : mem.HomePhone.Replace(",", "");
                            theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + filteredAddress + "," + filteredCity + "," + filteredState + "," + mem.Zip + "," + filteredPhone + "," + mem.EmailAddress + "," + mem.DateJoined + "," + mem.AuthorizedBy);
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "DeletedMembers.csv");
                case 6:
                    var theEmailList = db.OKSwingMemberLists.Where(em => em.EmailAddress != null && em.EmailAddress != string.Empty).OrderBy(o => o.LastName);
                    try
                    {
                        theFileContents.AppendLine("LastName,FirstName,Email");
                        foreach(OKSwingMemberList mem in theEmailList)
                        {
                            theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + mem.EmailAddress);
                        }
                    }
                    catch { }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "EmailList.csv");
                case 7:
                    var expiredMembers = db.OKSwingMemberLists.Where(an => an.Anniversary <= DateTime.Now && an.EmailAddress != null && an.EmailAddress != string.Empty).OrderByDescending(a => a.Anniversary);
                    theFileContents.AppendLine("LastName,FirstName,Anniversary,Email");
                    foreach(OKSwingMemberList mem in expiredMembers)
                    {
                        theFileContents.AppendLine(mem.LastName + "," + mem.FirstName + "," + mem.Anniversary + "," + mem.EmailAddress);
                    }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "ExpiredMembers.csv");
                case 8:
                    break;
                case 9:
                    var greenDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Green);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in greenDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",GREEN");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "GreenDancers.csv");
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
                    var pinkDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Pink);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in pinkDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",PINK");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "PinkDancers.csv");
                case 17:
                    var purpleDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Purple);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in purpleDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",PURPLE");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "PurpleDancers.csv");
                case 18:
                    break;
                case 19:
                    break;
                case 20:
                    var teacherDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Teacher);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in teacherDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",TEACHER");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "Teachers.csv");
                case 21:
                    break;
                case 22:
                    break;
                case 23:
                    break;
                case 24:
                    break;
                case 25:
                    var unknownDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Unknown);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in unknownDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",UNKNOWN");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "UnknownDancers.csv");
                case 26:
                    break;
                case 27:
                    break;
                case 28:
                    break;
                case 29:
                    var yellowDancers = db.OKSwingMemberLists.Where(bd => bd.ClassID == (int)ColorLevel.Yellow);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in yellowDancers)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",YELLOW");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "YellowDancers.csv");
                case 30:
                    var floorView = db.OKSwingMemberLists.Where(f => f.ClassID == (int)ColorLevel.FloorRentalOnly).OrderBy(of => of.LastName);
                    try
                    {
                        theFileContents.AppendLine("Full Name,Class Level");
                        foreach (OKSwingMemberList mem in floorView)
                        {
                            theFileContents.AppendLine(mem.LastName + " " + mem.FirstName + ",FLOOR RENTAL ONLY");
                        }
                    }
                    catch { break; }
                    return File(new System.Text.UTF8Encoding().GetBytes(theFileContents.ToString()), "text/csv", "FloorRentalOnly.csv");
                default:
                    break;
            }
            return null;
        }
    }
}
