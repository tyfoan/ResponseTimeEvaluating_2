using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using HtmlAgilityPack;
using MvcSiteMapProvider.Linq;
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
        public ActionResult EvaluateSite(Site site)
        {
            if (site != null)
            {
                var myUri = new Uri(site.SiteUrl);

                context.Sites.Add(new Site() { Host = myUri.Host, SiteUrl = myUri.AbsoluteUri });

                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = new HtmlDocument();
                doc = hw.Load(site.SiteUrl);


                for (int i = 0; i <= 10; i++)
                {

                    var request = WebRequest.Create(site.SiteUrl);
                    var timer = new Stopwatch();


                    timer.Start();
                    var response = (HttpWebResponse)request.GetResponse();
                    response.Close();
                    timer.Stop();
                    TimeSpan timeSpan = timer.Elapsed;


                    if (doc.DocumentNode.SelectNodes("//a[@href]").Count <= i)
                    {
                        break;
                    }
                    string href = doc.DocumentNode.SelectNodes("//a[@href]")[i].GetAttributeValue("href", string.Empty);
                    if (href != "/")
                    {
                        context.Responses.Add(new Response()
                        {
                            ResponseUrl = href,
                            SiteUrl = myUri.AbsoluteUri,
                            ResponseTime = timeSpan.Milliseconds,
                        });
                    }

                }
                context.SaveChanges();


            }
            return PartialView("_EvaluateSite", context.Responses);
        }

    }
}