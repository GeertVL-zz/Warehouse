using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransportAPI.Model
{
  public class Carrier
  {
    public int Id { get; set; }
    public int CarrierType { get; set; }
    public int Dwh { get; set; }
    public double Weight { get; set; }
    public double TarraWeight { get; set; }
    public double Volume { get; set; }
    public int LocM { get; set; }
    public int ColliNr { get; set; }
  }
}
