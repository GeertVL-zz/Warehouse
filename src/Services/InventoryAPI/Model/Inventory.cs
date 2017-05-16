using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Model
{
  public class Inventory
  {
    public int Id { get; set; }
    public int LocM { get; set; }
    public Product Product { get; set; }
    public double Quantity { get; set; }    
    public double Reserved { get; set; }
    public double Blocked { get; set; }
    public string BlockedReason { get; set; }
    public DateTime ExpiredOn { get; set; }    
  }
}
