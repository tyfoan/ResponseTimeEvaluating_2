using System.Web.Mvc;
using WebApplication1.EF;

namespace WebApplication1.Controllers
{
    public class AboutInnerController : Controller
    {
        ContextEvaluator e = new ContextEvaluator();
        // GET: LearnMore
        public ActionResult AboutInner_1()
        {
            return View();
        }
        public ActionResult AboutInner_2()
        {
            return View();
        }
        public ActionResult AboutInner_3()
        {
            return View();
        }
    }
}