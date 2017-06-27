using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.BL
{
  public class SecretSantaBl : ISecretSantaBl
  {
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
    
    
    
    
  }
}
