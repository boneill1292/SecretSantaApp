using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;

namespace SecretSantaApp.Controllers
{
  public class GroupsController : Controller
  {
    private readonly ISecretSantaBl _secretSantaBl;

    public GroupsController(ISecretSantaBl secretSantaBl)
    {
      _secretSantaBl = secretSantaBl;
    }

    [HttpGet]
    [Authorize]
    // [Route("benapp/test")]
    public IActionResult Index()
    {
      //var model = _secretSantaBl.DefaultTestDataViewModel();
      return View("Index");
    }

  }
}
