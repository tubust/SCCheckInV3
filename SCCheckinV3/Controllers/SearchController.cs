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
    public class SearchController : Controller
    {
        private SCCheckInEntities db = new SCCheckInEntities();

        // GET: Search
        // whoCalled Index
        // 0 (Default) = Add Check In Record
        // 1 = Edit Dancer
        // 2 = Edit Key Tag
        public ActionResult Index(int whoCalled = 0)
        {
            return View();
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
