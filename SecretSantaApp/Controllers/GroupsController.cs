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
using Microsoft.Extensions.Logging;
using SecretSantaApp.BL;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;
using Serilog;

namespace SecretSantaApp.Controllers
{
  [Authorize]
  public class GroupsController : Controller
  {
    private readonly ISecretSantaBl _secretSantaBl;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GroupsController> _log;

    public GroupsController(ISecretSantaBl secretSantaBl, IHttpContextAccessor httpContextAccessor, ILogger<GroupsController> log)
    {
      _secretSantaBl = secretSantaBl;
      _httpContextAccessor = httpContextAccessor;
      _log = log;
    }

    [HttpGet]
    // [Route("benapp/test")]
    public ActionResult Index()
    {
      //var user = await _userManager.GetUserAsync(HttpContext.User);
      //var LoggedInUser => User.Identity;

      //gets the id... dunno what to do with it
      //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      //var model = _secretSantaBl.DefaultTestDataViewModel();
      //var test = HttpContext.Session.GetObjectFromJson<TestDataViewModel>("Test");
      //var user = HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");

      var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
      //var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usermodel);



      var model = _secretSantaBl.DefaultGroupAdminModel();
      return View("Index", model);
    }

    [HttpGet]
    public ActionResult NewGroupPage()
    {
      var model = _secretSantaBl.DefaultGroupEditModel();
      return View("NewGroup", model);
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
        //save the new group
        var m = _secretSantaBl.SaveNewGroup(model);

        //get the group homepage data
        var grouphome = _secretSantaBl.GroupHomeEditModelByGroupId(m.GroupId);
        grouphome.InviteUsersCollection = _secretSantaBl.InviteUsersCollectionModelByAmountToGet(4);


        //var result = _secretSantaBl.DefaultGroupAdminModel();
        grouphome.NewGroup = true;
        return View("GroupHome", grouphome);

      }
      catch (Exception ex)
      {
        ModelState.AddModelError("", ex.Message);
        _log.LogError(" - ", ex.Message);
      }
      return View("Index");
    }




    [HttpGet]
    public ActionResult JoinGroup(int id)
    {
      var user = HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");
      var usereditmodel = new CustomUserEditModel();
      usereditmodel.Update(user);

      _secretSantaBl.JoinGroupAsCustomUser(usereditmodel, id);


      var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usereditmodel);
      return View("MyGroups", usergroupsvm);
    }



    [HttpGet]
    [Authorize]
    // [Route("benapp/test")]
    public ActionResult MyGroups()
    {
      var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
      var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usermodel);


      return View("MyGroups", usergroupsvm);
    }


    [HttpGet]
    public ActionResult GroupHome(int id)
    {
      var model = _secretSantaBl.GroupHomeEditModelByGroupId(id);

      return View("GroupHome", model);
    }




    [HttpGet]
    public ActionResult GroupsUserDoesNotBelongTo()
    {
      var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
      var model = _secretSantaBl.JoinGroupEditModelByAccountNumberString(usermodel.AccountNumberString);

      return View("JoinGroups", model);
    }


    [HttpGet]
    public ActionResult GetInviteUsersFields(int count)
    {
     // var model = _secretSantaBl.InviteUsersCollectionModelByAmountToGet(count);
      var model = _secretSantaBl.AdditionalInviteUsersViewModel(count);
      return PartialView("_InviteUsersRow", model);
    }



    [HttpPost]
    public ActionResult SendInvitesTousers(GroupHomeEditModel model)
    {
      var m = new Group();
      m.Update(model);
      return View("index");
    }


    [HttpGet]
    public ActionResult PromptUserForPassword()
    {
      return PartialView("_JoinGroupEntry");
    }

    [HttpPost]
    public ActionResult CheckPasswordInput()
    {
      return PartialView("_JoinGroupEntry");
    }

  }
}
