using Microsoft.EntityFrameworkCore;

namespace IntegrationEventLogEF
{
  public class IntegrationEventLogContext : DbContext
  {
    public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options)
      : base(options)
    {
      
    }

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }
  }
}