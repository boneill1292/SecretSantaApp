using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;

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


    public void SaveANewGroup()
    {
      var test = new Group();
      test.GroupName = "ben";

      _groupRepository.CreateGroup(test);
    }
    
    
  }
}
