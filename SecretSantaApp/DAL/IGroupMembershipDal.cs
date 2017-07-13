using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
