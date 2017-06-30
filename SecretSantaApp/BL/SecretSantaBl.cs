using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.BL
{
  public class SecretSantaBl : ISecretSantaBl
  {

    private readonly IGroupDal _groupDal;
    private readonly ICustomUserDal _customUserDal;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGroupMembershipDal _groupMembershipDal;
    public SecretSantaBl(IGroupDal groupDal,
                         ICustomUserDal customUserDal,
                         IHttpContextAccessor httpContextAccessor,
                          IGroupMembershipDal groupMembershipDal)
    {
      _groupDal = groupDal;
      _customUserDal = customUserDal;
      _httpContextAccessor = httpContextAccessor;
      _groupMembershipDal = groupMembershipDal;
    }

    public CustomUserEditModel CustomUserModelByLoggedInUser(ClaimsPrincipal user)
    {
      //var test = HttpContext.User.Identity.GetUserId();
      var result = new CustomUserEditModel();
      var acctid = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
      var name = user.Identity.Name;
      var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

      result.AccountNumberString = acctid;
      result.FullName = name;
      result.Email = email;

      return result;
    }

    public TestDataViewModel DefaultTestDataViewModel()
    {
      var result = new TestDataViewModel();
      return result;
    }

    public GroupAdminModel DefaultGroupAdminModel()
    {
      var result = new GroupAdminModel();
      result.ActiveGroups = _groupDal.AllActiveGroups();

      return result;
    }


    public GroupEditModel DefaultGroupEditModel()
    {
      var result = new GroupEditModel();
      return result;
    }

    public GroupEditModel SaveNewGroup(GroupEditModel model)
    {
      if (model.GroupName == null)
      {
        throw new Exception("Name is Required");
      }

      var groups = _groupDal.GroupByGroupName(model.GroupName);

      if (groups.Count > 0)
      {
        throw new Exception("This group already exists");
      }


      _groupDal.CreateGroup(model);

      model.Saved = true;
      return model;
    }


    public CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model)
    {
      //First check to see if this user exists
      //_customUserDal.SaveUser(model);
      var exists = _customUserDal.CustomUserByAccountNumber(model.AccountNumberString);

      if (exists != null)
      {
        model.UserId = exists.UserId;
        return model;
      }
      else
      {
        //Create a new user
        var newuser = _customUserDal.SaveUser(model);
        model.UserId = newuser.UserId;
      }

      return model;
      //var result = new CustomUserEditModel();
      //return result;
    }


    public void JoinGroupAsCustomUser(CustomUserEditModel user, int groupid)
    {

      var gmd = new GroupMembershipEditModel();
      gmd.AccountNumberString = user.AccountNumberString;
      gmd.GroupId = groupid;

      _groupMembershipDal.SaveMemberToGroup(gmd);

    }


    public MyGroupsViewModel MyGroupsViewModelByUserId(CustomUserEditModel user)
    {
      var result = new MyGroupsViewModel();
      result.MyGroups = new List<Group>();
      var grouplist = new List<Group>();

      var groupsibelongto = _groupMembershipDal.GroupsBelongingToUserAccountNumberString(user.AccountNumberString);

      foreach (var g in groupsibelongto)
      {
        var group = new Group();
        group = _groupDal.GetGroupById(g.GroupId);
        grouplist.Add(group);
      }

      result.MyGroups = grouplist;

      return result;
    }



    public GroupHomeEditModel GroupHomeEditModelByGroupId(int groupid)
    {
      var result = new GroupHomeEditModel();
      var userlist = new List<CustomUser>();

      var group = _groupDal.GetGroupById(groupid);

      result.Update(group);

      var groupmembership = _groupMembershipDal.AllGroupMembersByGroupId(groupid);

      foreach (var g in groupmembership)
      {
        var user = new CustomUser();

        user = _customUserDal.CustomUserByAccountNumber(g.AccountNumberString);
        userlist.Add(user);
      }

      result.GroupMembers = userlist;
      
      
      
      return result;
    }

  }
}
