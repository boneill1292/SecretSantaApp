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

    public CustomUser CustomUserModelByLoggedInUser(ClaimsPrincipal user)
    {
      //var test = HttpContext.User.Identity.GetUserId();
      var result = new CustomUser();
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


      //Need to play with my check user to see if i should add the person there or not.
      var testuser = _httpContextAccessor.HttpContext.User;
      var user = CustomUserModelByLoggedInUser(testuser);
      //var acctid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

      model.Active = true;
      model.InsertedDateTime = DateTime.Now;
      //var user = HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");
      //model.InsertedBy = 

      _groupRepository.CreateGroup(model);

      model.Saved = true;
      return model;
    }


    public CustomUser CheckUserByCustomUserAccountNumber(CustomUser model)
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




  }
}
