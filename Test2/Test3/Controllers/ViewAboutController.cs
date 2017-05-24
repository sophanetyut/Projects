using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Test3.Controllers
{
    public class ViewAboutController : Controller
    {
        public ActionResult Index()
        {
            ViewData["s"] = "View About Data";
            return View ();
        }
    }
}
