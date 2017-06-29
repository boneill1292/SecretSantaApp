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
    //private readonly AppDbContext _appDbContext;
    //private readonly ShoppingCart _shoppingCart;


    //public OrderRepository(AppDbContext appDbContext, ShoppingCart shoppingCart)
    //{
    //  _appDbContext = appDbContext;
    //  _shoppingCart = shoppingCart;
    //}

    private readonly IGroupRepository _groupRepository;
    public SecretSantaBl(IGroupRepository groupRepository)
    {
      _groupRepository = groupRepository;
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


    public CustomUserEditModel CheckUserByUserId(string userid)
    {
      
      
      var result = new CustomUserEditModel();
      return result;
    }




  }
}
