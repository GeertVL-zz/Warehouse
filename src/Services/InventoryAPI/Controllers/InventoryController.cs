using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Infrastructure;
using Microsoft.Extensions.Options;
using InventoryAPI.IntegrationEvents;
using InventoryAPI.IntegrationEvents.Events;
using InventoryAPI.Model;
using InventoryAPI.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Controllers
{
  [Produces("application/json")]
  [Route("api/v1/Inventory")]
  public class InventoryController : Controller
  {
    private readonly InventoryContext _inventoryContext;
    private readonly IOptionsSnapshot<Settings> _settings;
    private readonly IInventoryIntegrationEventService _inventoryIntegrationEventService;
    private readonly IMapper _mapper;

    public InventoryController(InventoryContext context, IOptionsSnapshot<Settings> settings, IInventoryIntegrationEventService inventoryIntegrationEventService, IMapper mapper)
    {
      _inventoryContext = context;
      _settings = settings;
      _inventoryIntegrationEventService = inventoryIntegrationEventService;
      _mapper = mapper;

      context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    [HttpGet]
    [Route("[action]/withname/{name:minlength(1)}")]
    public async Task<IActionResult> Items(string name)
    {
      var items = await _inventoryContext.Inventories.Include("Product")
        .Where(inv => inv.Product.Name.StartsWith(name))
        .ToListAsync();

      var model = _mapper.Map<List<Inventory>, List<StockViewModel>>(items);

      return Ok(model);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> UpdateInventory([FromBody] Inventory inventoryToUpdate)
    {
      var inventory = await _inventoryContext.Inventories.SingleOrDefaultAsync(i => i.Id == inventoryToUpdate.Id);
      if (inventory == null) return NotFound();
      var raiseProductQuantityChangedEvent = inventory.Quantity.Equals(inventoryToUpdate.Quantity);
      var oldQuantity = inventory.Quantity;

      inventory = inventoryToUpdate;
      _inventoryContext.Inventories.Update(inventory);

      if (raiseProductQuantityChangedEvent)
      {
        var quantityChangedEvent =
          new ProductQuantityChangedIntegrationEvent(inventory.Id, inventoryToUpdate.Quantity, oldQuantity);
        await _inventoryIntegrationEventService.SaveEventAndInventoryContextChangesAsync(quantityChangedEvent);
        await _inventoryIntegrationEventService.PublishThroughEventBusAsync(quantityChangedEvent);
      }
      else
      {
        await _inventoryContext.SaveChangesAsync();
      }

      return Ok();
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateInventory([FromBody] Inventory inventory)
    {
      _inventoryContext.Inventories.Add(
        new Inventory
        {
          Quantity = inventory.Quantity,
          Blocked = inventory.Blocked,
          BlockedReason = inventory.BlockedReason,
          ExpiredOn = inventory.ExpiredOn,
          LocM = inventory.LocM,
          Reserved = inventory.Reserved,
          Product = new Product
          {
            Name = inventory.Product.Name,
            Description = inventory.Product.Description,
            Height = inventory.Product.Height,
            Length = inventory.Product.Length,
            Price = inventory.Product.Price,
            ProductGroup = inventory.Product.ProductGroup,
            Supplier = inventory.Product.Supplier,
            UnitMeasure = inventory.Product.UnitMeasure,
            Volume = inventory.Product.Volume,
            Weight = inventory.Product.Weight,
            Width = inventory.Product.Width
          }
        }
      );
      await _inventoryContext.SaveChangesAsync();

      return Ok();
    }
  }
}
