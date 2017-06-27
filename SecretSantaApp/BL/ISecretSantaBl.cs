using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.BL
{
    public interface ISecretSantaBl
    {
      string TestStringMethod();

      TestDataViewModel DefaultTestDataViewModel();
    }
}
