using System.Data.Entity;
using WebApplication1.Models;

namespace WebApplication1.EF
{
    public class ContextEvaluator : DbContext
    {
        public DbSet<ResponseViewModel> Responses { get; set; }
        public ContextEvaluator() : base("ResponseEvaluator")
        {            
        }
    }
}