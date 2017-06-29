using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.BL
{
  public interface ISecretSantaBl
  {
    string TestStringMethod();

    TestDataViewModel DefaultTestDataViewModel();

    GroupAdminModel DefaultGroupAdminModel();

    GroupEditModel DefaultGroupEditModel();

    GroupEditModel SaveNewGroup(GroupEditModel model);

    CustomUserEditModel CheckUserByUserId(string userid);
  }
}
