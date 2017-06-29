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
  public interface ISecretSantaBl
  {
    CustomUser CustomUserEditModelByLoggedInUser(ClaimsPrincipal user);

    TestDataViewModel DefaultTestDataViewModel();

    GroupAdminModel DefaultGroupAdminModel();

    GroupEditModel DefaultGroupEditModel();

    GroupEditModel SaveNewGroup(GroupEditModel model);

    CustomUser CheckUserByCustomUserAccountNumber(CustomUser model);
  }
}
