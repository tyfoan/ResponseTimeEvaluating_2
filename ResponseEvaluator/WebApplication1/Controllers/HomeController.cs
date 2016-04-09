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


        private bool IsWellFormedUri(Uri url)
        {
            string patternDigit = @"\/?[0-9]+[0-9]\/?";
            string patternParam = @"\/\?";
            string patternAnchor = @"\#";

            if (url.Host == url.Host && url.Scheme != Uri.UriSchemeMailto && url.Segments.Length < 3
                    && !Regex.IsMatch(url.AbsoluteUri, patternDigit)
                    && !Regex.IsMatch(url.AbsoluteUri, patternAnchor)
                    && !Regex.IsMatch(url.AbsoluteUri, patternParam))
            {
                return true;

            }
            else
            {
                return false;
            }
        }


        private List<Uri> SitemapNodes(Uri url)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();      //init an empty document
            doc = hw.Load(url.Scheme + "://" + url.Host);

            var siteMapPart= new List<Uri>()
            {
                   new Uri(url.Scheme + "://" + url.Host) //siteMap - init root of sitemap
            };

            Uri nodeSiteMap;
            for (int i = 0; i < doc.DocumentNode.SelectNodes("//a[@href]").Count; i++)
            {
                string href = doc.DocumentNode.SelectNodes("//a[@href]")[i].GetAttributeValue("href", string.Empty);

                if (href.IndexOf("//") == 0)
                    href = url.Scheme + "://" + href.Remove(0, 2);


                if (Uri.IsWellFormedUriString(href, UriKind.Relative))
                {
                    nodeSiteMap = new Uri(url.Scheme + "://" + url.Host + href);

                    if (!siteMapPart.Contains(nodeSiteMap) && IsWellFormedUri(nodeSiteMap))
                        siteMapPart.Add(new Uri(nodeSiteMap.AbsoluteUri));
                }
                else if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    nodeSiteMap = new Uri(href);

                    if (!siteMapPart.Contains(nodeSiteMap) && IsWellFormedUri(nodeSiteMap))
                        siteMapPart.Add(new Uri(nodeSiteMap.AbsoluteUri));
                }
            }
            return siteMapPart;
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
            }
        }

        [HttpPost]
        public ActionResult BuildSiteMap(Site entryUrl)
        {
            var url = new Uri(entryUrl.SiteUrl);

            if (context.Sites.FirstOrDefault(x => x.Host == url.Host) != null)
            {
                var responses = context.Responses.Where(x => x.Host == url.Host);
                Uri uri;
                foreach (var item in responses)
                {
                    uri = new Uri(item.ResponseUrl);
                    item.ResponseTime = ResponseTimeEvaluation(uri);

                    context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                context.SaveChanges();
                return PartialView("_BuildSiteMap", context.Responses.Where(x => x.Host == url.Host));
            }
            else
            {
                SitemapNodes(url);
                List<Uri> sitemap = SitemapNodes(url);

                foreach (var item in SitemapNodes(url))
                    sitemap.Union(SitemapNodes(url));


                context.Sites.Add(new Site() { Host = sitemap[0].Host, SiteUrl = sitemap[0].AbsoluteUri });
                int time;
                foreach (var x in sitemap)
                {
                    time = ResponseTimeEvaluation(x);
                    if (time != 0)
                    {
                        context.Responses.Add(new Response() { Host = x.Host, ResponseUrl = x.AbsoluteUri, ResponseTime = time });
                    }
                }
                context.SaveChanges();
                return PartialView("_BuildSiteMap", context.Responses.Where(x => x.Host == url.Host));
            }
        }
    }
}