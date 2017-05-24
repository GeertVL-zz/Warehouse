using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.IntegrationEvents.Events
{
  public class ProductQuantityChangedIntegrationEvent : IntegrationEvent
  {
    public int InventoryId { get; private set; }
    public double NewQuantity { get; private set; }
    public double OldQuantity { get; private set; }

    public ProductQuantityChangedIntegrationEvent(int inventoryId, double newQuantity, double oldQuantity)
    {
      InventoryId = inventoryId;
      NewQuantity = newQuantity;
      OldQuantity = oldQuantity;
    }
  }
}
