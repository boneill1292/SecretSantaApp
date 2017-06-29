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

    private readonly IGroupRepository _groupRepository;
    private readonly ICustomUserRepository _customUserRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SecretSantaBl(IGroupRepository groupRepository,
                         ICustomUserRepository customUserRepository,
                         IHttpContextAccessor httpContextAccessor)
    {
      _groupRepository = groupRepository;
      _customUserRepository = customUserRepository;
      _httpContextAccessor = httpContextAccessor;
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
      result.ActiveGroups = _groupRepository.AllActiveGroups();
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

      var groups = _groupRepository.GroupByGroupName(model.GroupName);

      if (groups.Count > 0)
      {
        throw new Exception("This group already exists");
      }


      _groupRepository.CreateGroup(model);

      model.Saved = true;
      return model;
    }


    public CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model)
    {
      //First check to see if this user exists
      //_customUserRepository.SaveUser(model);
      var exists = _customUserRepository.CustomUserByAccountNumber(model.AccountNumberString);

      if (exists != null)
      {
        model.UserId = exists.UserId;
        return model;
      }
      else
      {
        //Create a new user
        var newuser = _customUserRepository.SaveUser(model);
        model.UserId = newuser.UserId;
      }

      return model;
      //var result = new CustomUserEditModel();
      //return result;
    }


    public void JoinGroupAsCustomUser(CustomUserEditModel user, int groupid)
    {
      var model = user;
        //now that we in here with the logged in user - lets add them to the group
    }



  }
}
