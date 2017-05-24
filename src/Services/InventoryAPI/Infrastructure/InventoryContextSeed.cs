using InventoryAPI.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Infrastructure
{
  public class InventoryContextSeed
  {
    public static async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int? retry = 0)
    {
      var context = (InventoryContext)applicationBuilder
        .ApplicationServices.GetService(typeof(InventoryContext));

      context.Database.Migrate();

      if (!context.Inventories.Any())
      {
        context.Inventories.AddRange(GetPreconfiguredInventories());
        await context.SaveChangesAsync();
      }
      if (!context.Products.Any())
      {
        context.Products.AddRange(GetPreconfiguredProducts());
        await context.SaveChangesAsync();
      }
    }

    static IEnumerable<Product> GetPreconfiguredProducts()
    {
      return new List<Product>
      {
        new Product { Name = "Les Paul", Supplier = "Gibson", Price = 1566, InventoryRef = 1 },
        new Product { Name = "SG100", Supplier = "Gibson", Price = 1377, InventoryRef = 2 } ,
        new Product { Name = "Telecaster", Supplier = "Fender", Price = 1456, InventoryRef = 3 },
        new Product { Name = "Stratocaster", Supplier = "Fender", Price = 1487, InventoryRef = 4 },
        new Product { Name = "Falcon", Supplier = "Gretsch", Price = 2987, InventoryRef = 5 }
      };
    }

    static IEnumerable<Inventory> GetPreconfiguredInventories()
    {
      return new List<Inventory>
      {
        new Inventory { LocM = 1, Quantity = 5 },
        new Inventory { LocM = 1, Quantity = 2 },
        new Inventory { LocM = 1, Quantity = 3 },
        new Inventory { LocM = 1, Quantity = 11 },
        new Inventory { LocM = 1, Quantity = 1 }
      };
    }
  }
}
