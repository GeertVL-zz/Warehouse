﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LocationAPI.Controllers
{
    public class HomeController : Controller
    {
      public IActionResult Index()
      {
        return new RedirectResult("~/swagger/ui");
      }
    }
}
