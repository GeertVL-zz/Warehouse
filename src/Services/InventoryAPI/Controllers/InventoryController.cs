using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Infrastructure;
using Microsoft.Extensions.Options;
using InventoryAPI.IntegrationEvents;
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

      var config = new MapperConfiguration(cfg => cfg.CreateMap<Inventory, StockViewModel>());
      

    }

    [HttpGet]
    [Route("[action]/withname/{name:minlength(1)}")]
    public async Task<IActionResult> Items(string name)
    {
      var items = await _inventoryContext.Inventories
        .Where(inv => inv.Product.Name.StartsWith(name))
        .ToListAsync();

      var model = _mapper.Map<List<Inventory>, List<StockViewModel>>(items);

      return Ok(model);
    }
  }
}
