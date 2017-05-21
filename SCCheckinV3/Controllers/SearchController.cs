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
        // 3 = Add Key Tag
        public ActionResult Index(int whoCalled = 0)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int whoCalled, int? memberID)
        {
            switch (whoCalled)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
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
