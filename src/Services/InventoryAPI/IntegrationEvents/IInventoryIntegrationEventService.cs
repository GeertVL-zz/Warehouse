using System;
using System.Data.Common;
using EventBus.Events;
using System.Threading.Tasks;
using EventBus.Abstractions;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;
using InventoryAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryAPI.IntegrationEvents
{
  public interface IInventoryIntegrationEventService
  {
    Task SaveEventAndInventoryContextChangesAsync(IntegrationEvent evt);
    Task PublishThroughEventBusAsync(IntegrationEvent evt);
  }

  public class InventoryIntegrationEventService : IInventoryIntegrationEventService
  {
    private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
    private readonly IEventBus _eventBus;
    private readonly InventoryContext _inventoryContext;
    private readonly IIntegrationEventLogService _eventLogService;

    public InventoryIntegrationEventService(IEventBus eventBus, InventoryContext inventoryContext, 
      Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
    {
      _inventoryContext = inventoryContext ?? throw new ArgumentNullException(nameof(inventoryContext));
      _integrationEventLogServiceFactory = integrationEventLogServiceFactory ??
                                           throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
      _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
      _eventLogService = _integrationEventLogServiceFactory(_inventoryContext.Database.GetDbConnection());
    }

    public async Task SaveEventAndInventoryContextChangesAsync(IntegrationEvent evt)
    {
      await ResilientTransaction.New(_inventoryContext)
        .ExecuteAsync(async () =>
        {
          await _inventoryContext.SaveChangesAsync();
          await _eventLogService.SaveEventAsync(evt, _inventoryContext.Database.CurrentTransaction.GetDbTransaction());
        });
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
    {
      _eventBus.Publish(evt);
      await _eventLogService.MarkEventAsPublishedAsync(evt);
    }
  }
}
