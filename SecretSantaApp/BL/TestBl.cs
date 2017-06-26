using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.BL
{
  public class TestBl : ITestBl
  {
    public string TestStringMethod()
    {
      var result = "sup dog";

      return result;
    }
  }
}
