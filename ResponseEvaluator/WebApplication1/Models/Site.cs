using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Site
    {
        [Key]
        public int SiteId { get; set; }
        public string SiteUrl { get; set; }
        public string Host { get; set; }

        public ICollection<Response> Response { get; set; }
    }
}