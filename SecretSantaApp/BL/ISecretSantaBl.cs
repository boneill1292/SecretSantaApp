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

    string UserFullNameByAccountNumberString(string acctno);
    string UserImageByAccountNumberString(string acctno);

    GroupAdminModel DefaultGroupAdminModel();

    GroupEditModel DefaultGroupEditModel();

    GroupEditModel SaveNewGroup(GroupEditModel model);

    CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model);

    void JoinGroupAsCustomUser(CustomUserEditModel user, int groupid);

    MyGroupsViewModel MyGroupsViewModelByUserId(CustomUserEditModel user);

    GroupHomeEditModel GroupHomeEditModelByGroupId(int groupid);

    JoinGroupEditModel JoinGroupEditModelByAccountNumberString(string acctno);

    InviteUsersCollectionModel InviteUsersCollectionModelByAmountToGet(int amount);

    InviteUsersViewModel AdditionalInviteUsersViewModel(int tempid);

    JoinGroupEditModel JoinGroupEditModelByGroupId(int groupid);

    JoinGroupEditModel CheckPasswordInput(JoinGroupEditModel model);


    GroupRulesEditModel NewRuleEditModelByGroupId(int groupid);
    GroupRulesEditModel GroupRuleEditModelByRuleId(int ruleid);

    GroupRulesEditModel SaveGroupRules(GroupRulesEditModel model);

    GroupRulesEditModel DeleteGroupRule(GroupRulesEditModel model);
    GroupRulesDisplayModel GroupRulesDisplayModelByGroupId(int groupid);

    GroupChatDisplayModel GroupChatDisplayModelByGroupId(int groupid);

    GroupMessageEditModel NewGroupMessageEditModelByGroupId(int groupid);

    GroupMessageEditModel SaveGroupMessage(GroupMessageEditModel model);

    MemberConditionsEditModel MemberConditionsEditModelByMembershipId(int membershipid);
  }
}
