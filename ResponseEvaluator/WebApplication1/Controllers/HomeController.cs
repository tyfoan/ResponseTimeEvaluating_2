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

                    if (newUri.Host == myUri.Host && !siteMap.Contains(newUri) && newUri.Scheme != Uri.UriSchemeMailto
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

                    if (newUri.Host == myUri.Host && !siteMap.Contains(newUri) && newUri.Scheme != Uri.UriSchemeMailto
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternDigit)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternAnchor)
                        && !Regex.IsMatch(newUri.AbsoluteUri, patternParam))
                    {
                        siteMap.Add(new Uri(newUri.AbsoluteUri));
                    }
                }
               
            }
            return siteMap;
        }

        private int ResponseTimeEvaluation(Uri url)
        {
            HttpWebResponse response = null;
            try
            {
                var request = WebRequest.Create(url.AbsoluteUri);
                var timer = new Stopwatch();

                timer.Start();
                response = (HttpWebResponse)request.GetResponse();

                response.Close();
                timer.Stop();

                TimeSpan timeSpan = timer.Elapsed;

                return (int)timeSpan.Milliseconds;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    Debug.Write("Errorcode: {0}", ((int)response.StatusCode).ToString());

                }
                else
                {
                    Debug.Write("Error: {0}", e.Status.ToString());
                }
                return 0;
                throw;
            }
        }

        [HttpPost]
        public ActionResult BuildSiteMap(Site site)
        {
            var tmpUri = new Uri(site.SiteUrl);
            if (context.Sites.FirstOrDefault(x => x.Host == tmpUri.Host) != null)
            {
                var responses = context.Responses.Where(x => x.Host == tmpUri.Host);
                Uri uri;
                foreach (var item in responses)
                {
                    uri = new Uri(item.ResponseUrl);
                    item.ResponseTime = ResponseTimeEvaluation(uri);

                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                context.SaveChanges();
                return PartialView("_BuildSiteMap", context.Responses.Where(x => x.Host == tmpUri.Host));
            }
            else
            {
                SitemapNodes(site.SiteUrl);
                List<Uri> sitemap = SitemapNodes(site.SiteUrl);

                foreach (var item in SitemapNodes(site.SiteUrl))
                    sitemap.Union(SitemapNodes(item.AbsoluteUri));


                context.Sites.Add(new Site() { Host = sitemap[0].Host, SiteUrl = sitemap[0].AbsoluteUri });
                int time;
                foreach (var x in sitemap)
                {
                    time = ResponseTimeEvaluation(x);
                    if (time != 0)
                    {
                        context.Responses.Add(new Response() { Host = x.Host, ResponseUrl = x.AbsoluteUri, ResponseTime = time  });
                    }
                }
                context.SaveChanges();
                return PartialView("_BuildSiteMap", context.Responses.Where(x => x.Host == tmpUri.Host));
            }
        }



        [HttpPost]
        public ActionResult EvaluateSite(string url, int id)
        {
            try
            {
                var myUri = new Uri(url);


                var tmp = context.Responses.Find(id);
                tmp.ResponseTime = ResponseTimeEvaluation(myUri);

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