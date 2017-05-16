using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return new RedirectResult("~/swagger/ui");
    }
  }
}
