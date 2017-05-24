using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test3.Controllers
{
    public class AboutController : Controller
    {
        public ActionResult Index()
        {
            ViewData["s"] = "About Controller Data";
            return View ("ViewData");
        }
    }
}