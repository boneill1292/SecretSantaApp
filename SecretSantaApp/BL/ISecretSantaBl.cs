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
    CustomUserEditModel CustomUserModelByLoggedInUser(ClaimsPrincipal user);

    TestDataViewModel DefaultTestDataViewModel();

    GroupAdminModel DefaultGroupAdminModel();

    GroupEditModel DefaultGroupEditModel();

    GroupEditModel SaveNewGroup(GroupEditModel model);

    CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model);
  }
}
