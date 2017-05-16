using System.Threading.Tasks;

namespace EventBus.Abstractions
{
  public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
  {
    Task Handle(TIntegrationEvent @event);
  }

  public interface IIntegrationEventHandler
  {
  }
}