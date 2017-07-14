﻿using System;
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
      //HttpContext.Session.SetObjectAsJson("LoggedInUser", usermodel);
      //var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usermodel);

      var model = _secretSantaBl.DefaultGroupAdminModel();
      return View("Index", model);
    }

    /// <summary>
    /// Used to link the user back to the groups they are a member of
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    // [Route("benapp/test")]
    public ActionResult MyGroups()
    {
      var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
      var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usermodel);


      return View("MyGroups", usergroupsvm);
    }



    /// <summary>
    /// Going to get used a lot. Used to generate the Group Home
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult GroupHome(int id)
    {
      var model = _secretSantaBl.GroupHomeEditModelByGroupId(id);
      model.InviteUsersCollection = _secretSantaBl.InviteUsersCollectionModelByAmountToGet(4);

      return View("GroupHome", model);
    }



    /// <summary>
    /// Get the NewGroup View. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult NewGroupPage()
    {
      var model = _secretSantaBl.DefaultGroupEditModel();
      return View("NewGroup", model);
    }


    /// <summary>
    /// Saves the new group that the user submitted.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
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


    /// <summary>
    /// This is the extra field used when we want to add more people to invite.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult GetInviteUsersFields(int count)
    {
      // var model = _secretSantaBl.InviteUsersCollectionModelByAmountToGet(count);
      var model = _secretSantaBl.AdditionalInviteUsersViewModel(count);
      return PartialView("_InviteUsersRow", model);
    }



    /// <summary>
    /// Called from the GroupHome View.
    /// This is note quite used yet. This will be coming soon.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SendInvitesTousers(GroupHomeEditModel model)
    {
      var m = new Group();
      m.Update(model);
      return View("Index");
    }



    /// <summary>
    /// Gets the Join Groups page
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult AvailableGroupsToJoin()
    {
      var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
      var model = _secretSantaBl.JoinGroupEditModelByAccountNumberString(usermodel.AccountNumberString);

      return View("JoinGroups", model);
    }


    /// <summary>
    /// Used to prompt the user for a password to join a group
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("secretsanta/joingroup/{id}")]
    public ActionResult PromptUserForPassword(int id)
    {
      var model = _secretSantaBl.JoinGroupEditModelByGroupId(id);
      return PartialView("_JoinGroupEntry", model);
    }


    /// <summary>
    /// checks to see if the users password was correct
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SubmitJoinGroup(JoinGroupEditModel model)
    {
      //We need to pass the correct password - if the user does that. Add them to the group, and load the group  homepage.
      try
      {
        var liu = _httpContextAccessor.HttpContext.User;
        var u = _secretSantaBl.CustomUserModelByLoggedInUser(liu);
        //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
        //var cu = new CustomUserEditModel();
        //cu.Update(u);

        model.CustomUser = u;
        var m = _secretSantaBl.CheckPasswordInput(model);
        //ModelState.Clear();
        //m.Verified = true;
        return PartialView("_JoinGroupEntry", m);
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex.Message);
      }
      return PartialView("_JoinGroupEntry", model);
    }


    [HttpGet]
    public ActionResult GetManageRulesPopup(int groupid)
    {
      var model = _secretSantaBl.NewRuleEditModelByGroupId(groupid);
      return PartialView("_NewRulePopup", model);
    }

    [HttpGet]
    public ActionResult EditRulePopup(int ruleid)
    {
      var model = _secretSantaBl.GroupRuleEditModelByRuleId(ruleid);
      return PartialView("_NewRulePopup", model);
    }

    [HttpGet]
    public ActionResult DeleteRulePopup(int ruleid)
    {
      var model = _secretSantaBl.GroupRuleEditModelByRuleId(ruleid);
      return PartialView("_DeleteRulePopup", model);
    }

    [HttpPost]
    public ActionResult DeleteGroupRule(GroupRulesEditModel model)
    {
      try
      {
        //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
        //var m = _secretSantaBl.SaveGroupRules(model);
        var m = _secretSantaBl.DeleteGroupRule(model);
        m.Saved = true;
        return PartialView("_NewRulePopup", m);
        //return PartialView("_JoinGroupEntry", m);
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex.Message);
      }
      return PartialView("_NewRulePopup", model);
    }



    [HttpPost]
    public ActionResult SaveGroupRule(GroupRulesEditModel model)
    {
      try
      {
        //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
        var m = _secretSantaBl.SaveGroupRules(model);
        m.Saved = true;
        return PartialView("_NewRulePopup", m);
        //return PartialView("_JoinGroupEntry", m);
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex.Message);
      }
      return PartialView("_NewRulePopup", model);
    }


    [HttpGet]
    public ActionResult GetGroupRulesPartial(int groupid)
    {
      var model = _secretSantaBl.GroupRulesDisplayModelByGroupId(groupid);
      return PartialView("_GroupRules", model);
    }


    [HttpGet]
    public ActionResult GetChatPartial(int groupid)
    {
      var model = _secretSantaBl.GroupChatDisplayModelByGroupId(groupid);
      return PartialView("_GroupChat", model);
    }



    [HttpGet]
    public ActionResult NewMessagePartial(int groupid)
    {
      var model = _secretSantaBl.NewGroupMessageEditModelByGroupId(groupid);
      return PartialView("_NewMessage", model);
    }


    [HttpPost]
    public ActionResult SaveGroupMessage(GroupMessageEditModel model)
    {
      try
      {
        //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
        // var m = _secretSantaBl.SaveGroupRules(model);
        var m = _secretSantaBl.SaveGroupMessage(model);
        m.Saved = true;
        return PartialView("_NewMessage", m);
        //return PartialView("_JoinGroupEntry", m);
      }
      catch (Exception ex)
      {
        _log.LogWarning(ex.Message);
      }
      return PartialView("_NewMessage", model);
    }


    [HttpGet]
    public ActionResult GetMemberConditionsPartial(int membershipid, string acctno)
    {
      var model = _secretSantaBl.MemberConditionsEditModelByMembershipId(membershipid, acctno);
      return PartialView("_MemberConditions", model);
    }


    [HttpPost]
    public ActionResult SaveMemberCondition(MemberConditionsEditModel model)
    {
      var m = _secretSantaBl.SaveMemberCondition(model);
      m.Saved = true;
      
 
      
      return PartialView("_MemberConditions", model);
    }




    ////Will not use
    //[HttpGet]
    //public ActionResult JoinGroup(int id)
    //{
    //  var user = HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");
    //  var usereditmodel = new CustomUserEditModel();
    //  usereditmodel.Update(user);
    //  _secretSantaBl.JoinGroupAsCustomUser(usereditmodel, id);
    //  var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usereditmodel);
    //  return View("MyGroups", usergroupsvm);
    //}
  }
}
