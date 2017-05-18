using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SCCheckinV3.Controllers
{
    public class QuickCheckInController : Controller
    {
        // GET: QuickCheckIn/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuickCheckIn/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Create");
            }
            catch
            {
                return View();
            }
        }
    }
}
