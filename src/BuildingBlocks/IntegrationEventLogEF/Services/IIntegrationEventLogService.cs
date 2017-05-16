using System.Data.Common;
using System.Threading.Tasks;
using EventBus.Events;

namespace IntegrationEventLogEF.Services
{
  public interface IIntegrationEventLogService
  {
    Task SaveEventAsync(IntegrationEvent @event, DbTransaction transaction);
    Task MarkEventAsPublishedAsync(IntegrationEvent @event);
  }
}
