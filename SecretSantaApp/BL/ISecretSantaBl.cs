﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        GroupEditModel GroupEditModelByGroupId(int id);

        GroupEditModel SaveNewGroup(GroupEditModel model);
        GroupEditModel SaveGroup(GroupEditModel model);

        CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model);

        void JoinGroupAsCustomUser(CustomUserEditModel user, int groupid);

        MyGroupsViewModel MyGroupsViewModelByUserId(CustomUserEditModel user);

        GroupHomeEditModel GroupHomeEditModelByGroupId(int groupid);

        JoinGroupEditModel JoinGroupEditModelByAccountNumberString(string acctno);

        InviteUsersEditModel InviteUsersEditModelByGroupId(int groupid);
        InviteUsersCollectionModel InviteUsersCollectionModelByAmountToGet(int amount, int groupid);

        InviteUsersViewModel AdditionalInviteUsersViewModel(int tempid, int groupid);

        Task<InviteUsersEditModel> SendInviteToUsersAsync(InviteUsersEditModel model);
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

        MemberConditionsEditModel MemberConditionsEditModelByMembershipId(int membershipid, string acctno);

        MemberConditionsEditModel SaveNewMemberCondition(MemberConditionsEditModel model);

        MemberConditionsEditModel MemberConditionsEditModelByConditionId(int conditionid);

        MemberConditionsEditModel DeleteMemberCondition(MemberConditionsEditModel model);

        SelectList OtherUsersDropDown(string acctnostr, int groupid);

        DrawNamesDisplayModel DrawNamesDisplayModelByGroupId(int groupid);

        Task<DrawNamesDisplayModel> DrawNamesAsync(DrawNamesDisplayModel model);

        GroupPairingDisplayModel GroupPairingDisplayModelByLoggedInUserByGroupId(int groupid);

        //helpers
        CustomUserEditModel GetLoggedInUser();

        void GetContextFromController(ContentResult content);

        string ConditionDisplayByAccountNumbers(string ur, string us);


        //acct
        UserProfileViewModel UserProfileViewModelByAcctNo(ClaimsPrincipal user);

        //CustomUserDetailsEditModel UserDetailsByUserAcctNo(string acctno);
        CustomUserDetailsEditModel UserDetailsEditModelByUserId(int userid);

        CustomUserDetailsEditModel UserDetailsEditModelByAcctNo(string acctno);

        CustomUserDetailsDisplayModel UserDetailsDisplayModelByAcctNo(string acctno);
        CustomUserDetailsEditModel CustomUserDetailsEditModelByAcctNo(string acctno);
        CustomUserDetailsEditModel SaveUserDetails(CustomUserDetailsEditModel model);


        List<SelectListItem> CommonSizesDropdown();
    }
}