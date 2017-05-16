using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Model
{
  public class Product
  {
    public int Id { get; set; }
    public int InventoryRef { get; set; }
    public int ProductGroup { get; set; }
    public string Name { get; set; }
    public string Supplier { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public double Weight { get; set; }
    public double Price { get; set; }
    public string UnitMeasure { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
  }
}
