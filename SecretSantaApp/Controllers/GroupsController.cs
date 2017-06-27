using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.Controllers
{
  [Authorize]
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
      var model = _secretSantaBl.DefaultGroupAdminModel();
      return View("Index", model);
    }

    [HttpGet]
    public IActionResult NewGroupPage()
    {
      var model = _secretSantaBl.DefaultGroupEditModel();
      return View("NewGroup",model);
    }

    [HttpPost]
    //[Route("tickets/{department}/newcategoryonticketview/edit")]
    public ActionResult SaveNewGroup(GroupEditModel model)
    {
      if (!ModelState.IsValid)
      {
        return View("NewGroup", model);
      }
      try
      {
        var m = _secretSantaBl.SaveNewGroup(model);
        return View("NewGroup",m);

      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
      }
      return View("Index");
    }



  }
}
