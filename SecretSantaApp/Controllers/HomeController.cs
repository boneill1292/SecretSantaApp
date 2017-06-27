using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.Controllers
{
  public class HomeController : Controller
  {
    //private readonly AppDbContext _appDbContext;
    //private readonly ShoppingCart _shoppingCart;


    //public OrderRepository(AppDbContext appDbContext, ShoppingCart shoppingCart)
    //{
    //  _appDbContext = appDbContext;
    //  _shoppingCart = shoppingCart;
    //}


    private readonly ISecretSantaBl _secretSantaBl;
    //private readonly AppDbContext _appDbContext;

    

    public HomeController(ISecretSantaBl secretSantaBl
                          //AppDbContext dbContext
                          )
    {
      _secretSantaBl = secretSantaBl;
     // _appDbContext = dbContext;
    }

    [HttpGet]
    // [Route("benapp/test")]
    public IActionResult Index()
    {
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
