using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.BL;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.Controllers
{
  [Authorize]
  public class GroupsController : Controller
  {
    private readonly ISecretSantaBl _secretSantaBl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupsController(ISecretSantaBl secretSantaBl, IHttpContextAccessor httpContextAccessor)
    {
      _secretSantaBl = secretSantaBl;
      _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    [Authorize]
    // [Route("benapp/test")]
    public IActionResult Index()
    {
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //var LoggedInUser => User.Identity;
      
      //gets the id... dunno what to do with it
      //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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
        var result = _secretSantaBl.DefaultGroupAdminModel();
        return View("Index", result);

      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
      }
      return View("Index");
    }



  }
}
