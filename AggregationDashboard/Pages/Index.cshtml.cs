using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AggregationDashboard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AggregationDbContext _context;

        public IndexModel(AggregationDbContext context)
        {
            _context = context;
        }

        public List<AggregatedResult> Results { get; set; }

        public void OnGet()
        {
            Results = _context.AggregatedResults
                .OrderByDescending(r => r.AggregationTime)
                .Take(50)
                .ToList();
        }
    }
}
