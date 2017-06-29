using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;
using SecretSantaApp.Models;
using Microsoft.AspNetCore.Http; // Needed for the SetString and GetString extension methods
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.Controllers
{
  public class HomeController : Controller
  {
  
    private readonly ISecretSantaBl _secretSantaBl;
    
    public HomeController(ISecretSantaBl secretSantaBl)
    {
      _secretSantaBl = secretSantaBl;
    }

    [HttpGet]
    // [Route("benapp/test")]
    public IActionResult Index()
    {
      //HttpContext.Session.SetString("Test", "Ben Rules!");  
      var test = new TestDataViewModel();
      test.GroupId = 123;
      test.GroupName = "sup";

      HttpContext.Session.SetObjectAsJson("Test", test);

      //var model = _secretSantaBl.DefaultTestDataViewModel();
      return View("Index");
    }

    [HttpPost]
    public IActionResult Checkout(TestDataViewModel model)
    {

      return View("Index");
    }

    public IActionResult About()
    {
      var test = HttpContext.Session.GetObjectFromJson<TestDataViewModel>("Test");
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
