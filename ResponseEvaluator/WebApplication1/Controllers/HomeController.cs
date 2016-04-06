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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.Entity.Infrastructure;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        ContextEvaluator context = new ContextEvaluator();

        public ActionResult Index()
        {
            return View();
        }

        private static void DisplayTrackedEntities(DbChangeTracker changeTracker)
        {
            Debug.WriteLine("");

            var entries = changeTracker.Entries();
            foreach (var entry in entries)
            {
                Console.WriteLine("Entity Name: {0}", entry.Entity.GetType().FullName);
                Console.WriteLine("Status: {0}", entry.State);
            }
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
        }



        private List<Uri> SitemapNodes(string url)
        {
            var myUri = new Uri(url);

            string patternDigit = @"\/?[0-9]+[0-9]\/?";
            string patternParam = @"\/\?";
            string patternAnchor = @"\#";

            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();      //init an empty document
            doc = hw.Load(myUri.Scheme + "://" + myUri.Host);

            var siteMap = new List<Uri>()
            {
                   new Uri(myUri.Scheme + "://" + myUri.Host) //siteMap - init root of sitemap
            };

            Uri newUri;
            for (int i = 0; i < doc.DocumentNode.SelectNodes("//a[@href]").Count; i++)
            {
                string href = doc.DocumentNode.SelectNodes("//a[@href]")[i].GetAttributeValue("href", string.Empty);

                if (href.IndexOf("//") == 0)
                {
                    href = myUri.Scheme + "://" + href.Remove(0, 2);
                }

                if (Uri.IsWellFormedUriString(href, UriKind.Relative))
                {
                    newUri = new Uri(myUri.Scheme + "://" + myUri.Host + href);

                    if (newUri.Host == myUri.Host && !siteMap.Contains(newUri) && newUri.Scheme != Uri.UriSchemeMailto && newUri.Segments.Length < 3
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternDigit)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternAnchor)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternParam))
                    {
                        siteMap.Add(new Uri(newUri.AbsoluteUri));
                    }
                }
                else if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    newUri = new Uri(href);

                    if (newUri.Host == myUri.Host && !siteMap.Contains(newUri) && newUri.Scheme != Uri.UriSchemeMailto && newUri.Segments.Length < 3
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternDigit)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternAnchor)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternParam))
                    {
                        siteMap.Add(new Uri(newUri.AbsoluteUri));
                    }
                }
                if (siteMap.Count >= 5)
                {
                    return siteMap;
                }
            }
            return siteMap;
        }

        public ActionResult EvaluateSite()
        {
            return Content("asd");
        }

        [HttpPost]
        public ActionResult BuildSiteMap(Site site)
        {

            if (context.Sites.FirstOrDefault(x => x.SiteUrl == site.SiteUrl) != null)
            {
                return PartialView("_BuildSiteMap", context.Responses);
            }
            else
            {
                SitemapNodes(site.SiteUrl);
                List<Uri> sitemap = SitemapNodes(site.SiteUrl);

                foreach (var item in SitemapNodes(site.SiteUrl))
                    sitemap.Union(SitemapNodes(item.AbsoluteUri));

                context.Sites.Add(new Site() { Host = sitemap[0].Host, SiteUrl = sitemap[0].AbsoluteUri });
                sitemap.ForEach(x => context.Responses.Add(new Response() { SiteUrl = x.AbsoluteUri, ResponseUrl = x.AbsoluteUri }));
                context.SaveChanges();

                return PartialView("_BuildSiteMap", context.Responses);
            }
        }



        [HttpPost]
        public ActionResult EvaluateSite(string url, int id)
        {
            try
            {
                var myUri = new Uri(url);

                var request = WebRequest.Create(url);
                var timer = new Stopwatch();

                timer.Start();
                var response = (HttpWebResponse)request.GetResponse();
                response.Close();
                timer.Stop();

                TimeSpan timeSpan = timer.Elapsed;

                var tmp = context.Responses.Find(id);
                tmp.ResponseTime = (int)timeSpan.Milliseconds;

                context.Entry(tmp).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return Json(context.Responses);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw;
            }
        }
    }
}