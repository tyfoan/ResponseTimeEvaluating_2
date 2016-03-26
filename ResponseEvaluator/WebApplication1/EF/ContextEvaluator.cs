using System.Data.Entity;
using WebApplication1.Models;

namespace WebApplication1.EF
{
    public class ContextEvaluator : DbContext
    {
        public DbSet<Response> Responses { get; set; }
        public DbSet<Site> Sites { get; set; }
        public ContextEvaluator() : base("ResponseEvaluator")
        {            
        }
    }
}