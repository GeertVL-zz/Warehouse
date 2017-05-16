using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InventoryAPI.Model;
using InventoryAPI.ViewModel;

namespace InventoryAPI.Infrastructure.Profiles
{
  public class InventoryProfile : Profile
  {
    public InventoryProfile()
    {
      CreateMap<Inventory, StockViewModel>();
    }
  }
}
