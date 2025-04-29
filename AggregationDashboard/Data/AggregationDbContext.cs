using Microsoft.EntityFrameworkCore;
using AggregationDashboard;

public class AggregationDbContext : DbContext
{
    public AggregationDbContext(DbContextOptions<AggregationDbContext> options)
        : base(options) { }

    public DbSet<AggregatedResult> AggregatedResults { get; set; }
}
