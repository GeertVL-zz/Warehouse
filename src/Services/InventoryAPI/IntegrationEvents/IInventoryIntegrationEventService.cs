using System.Threading.Tasks;
using EventBus.Events;

namespace InventoryAPI.IntegrationEvents
{
  public interface IInventoryIntegrationEventService
  {
    Task SaveEventAndInventoryContextChangesAsync(IntegrationEvent evt);
    Task PublishThroughEventBusAsync(IntegrationEvent evt);
  }
}
