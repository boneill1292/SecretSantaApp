using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.BL
{
  public class SecretSantaBl : ISecretSantaBl
  {

    private readonly IGroupRepository _groupRepository;
    private readonly ICustomUserRepository _customUserRepository;
    public SecretSantaBl(IGroupRepository groupRepository, ICustomUserRepository customUserRepository)
    {
      _groupRepository = groupRepository;
      _customUserRepository = customUserRepository;
    }

    public string TestStringMethod()
    {
      var result = "sup dog";
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
      //var acctid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

      model.Active = true;
      model.InsertedDateTime = DateTime.Now;
      // model.InsertedBy = H

      _groupRepository.CreateGroup(model);

      model.Saved = true;
      return model;
    }


    public CustomUserEditModel CheckUserByUserId(CustomUserEditModel model)
    {
      //First check to see if this user exists
      //_customUserRepository.SaveUser(model);
      var exists = _customUserRepository.CustomUserByAccountNumber(model.AccountNumber);

      if (exists)
      {
        //We can redirect here
        return model;
      }
      else
      {
        //Create a new user
        var newuser = _customUserRepository.SaveUser(model);
      }

      return model;
      //var result = new CustomUserEditModel();
      //return result;
    }




  }
}
