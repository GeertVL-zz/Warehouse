using System;
using System.Collections.Generic;
using System.Text;

namespace Location.Domain.AggregatesModel
{
  public class Location
  {
    public int Building { get; set; }
    public int Aisle { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int LocationType { get; set; }
    public int State { get; set; }
  }
}
