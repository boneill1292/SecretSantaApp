using System.Collections.Generic;

namespace SecretSantaApp.Models
{
    public interface IGroupMembershipDal
    {
        GroupMembership SaveMemberToGroup(GroupMembership g);
        GroupMembership GroupMembershipModelByGroupMembershipId(int membershipid);
        List<GroupMembership> GroupsBelongingToUserAccountNumberString(string acctno);

        List<GroupMembership> AllGroupMembersByGroupId(int groupid);

        List<GroupMembership> GroupsUserDoesNotBelongToByAccountNumberString(string acctno);
    }
}