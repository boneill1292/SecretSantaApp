using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;

namespace SecretSantaApp.Controllers
{
  public class HomeController : Controller
  {

    private readonly ITestBl _testBl;

    public HomeController(ITestBl testBl)
    {
      _testBl = testBl;
    }

    public IActionResult Index()
    {
      //var model = _testBl.TestStringMethod();
      return View("Index");
    }

    public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    public IActionResult Error()
    {
      return View();
    }
  }
}
