using System;

namespace InventoryAPI.Infrastructure.Exceptions
{
    public class InventoryDomainException : Exception
    {
      public InventoryDomainException()
      {
        
      }

      public InventoryDomainException(string message)
        : base(message)
      {
        
      }

      public InventoryDomainException(string message, Exception innerException)
        : base(message, innerException)
      {
        
      }
    }
}
