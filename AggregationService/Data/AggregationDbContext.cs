using AggregationService.Models;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Data
{
    public class AggregationDbContext : DbContext
    {
        public AggregationDbContext(DbContextOptions<AggregationDbContext> options) : base(options)
        {
        }

        public DbSet<AggregatedResult> AggregatedResults { get; set; }
    }
}
