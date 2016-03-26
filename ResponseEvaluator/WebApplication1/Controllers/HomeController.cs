using System;
using System.Diagnostics;
using System.Net;
using System.Web.Mvc;
using WebApplication1.EF;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
 
    public class HomeController : Controller
    {
        ContextEvaluator context = new ContextEvaluator();

        public ActionResult Index()
        {
            return View();
        }

        //    [HttpPost]
        //    public ActionResult Index(Site response)   same
        //    {
        //        return View();
        //}
        public ActionResult EvaluateSite()
        {
            return PartialView("_EvaluateSite", context.Responses);
        }

        [HttpPost]
        public ActionResult EvaluateSite(ResponseViewModel site)
        {
            if (site != null)
            {
                var request = WebRequest.Create(site.ResponseUrl);
                var timer = new Stopwatch();


                timer.Start();
                var response = (HttpWebResponse)request.GetResponse();
                response.Close();


                timer.Stop();
                TimeSpan timeSpan = timer.Elapsed;


                site.ResponseTime = timeSpan.Milliseconds;
                context.Responses.Add(site);
                context.SaveChanges();
            }
            return PartialView("_EvaluateSite", context.Responses);
        }


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