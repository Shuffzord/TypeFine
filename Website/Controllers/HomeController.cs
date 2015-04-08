using System.IO;
using System.Web.Mvc;
using Website.Models;
using Website.Views;

namespace Website.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index(SearchModel query)
        {
            return View(query);
        }

        public ActionResult Get()
        {
            return Redirect(ViewHelpers.GetWindowsStoreAddress());
        }

        public ActionResult GetAdsSetting()
        {
            var dir = Server.MapPath("~/Files");
            var path = Path.Combine(dir, "defaultAdSettings.xml");
            return File(path, "text/xml");
        }
    }
}