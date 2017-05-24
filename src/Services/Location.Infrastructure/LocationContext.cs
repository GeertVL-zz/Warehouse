using System;
using System.Threading;
using System.Threading.Tasks;
using Location.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Location.Infrastructure
{
    public class LocationContext : DbContext, IUnitOfWork
    {
      private const string DEFAULT_SCHEMA = "location";
      
      public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = new CancellationToken())
      {
        throw new NotImplementedException();
      }
    }
}
