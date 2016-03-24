using System.Web.Mvc;

namespace WebApplication1.Controllers
{
 
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
          
        //[HttpPost]                                                        
        //public ActionResult Index(Response response)   same
        //{
        //    return View();
        //}


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ContactInner_1()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ContactInner_2()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}