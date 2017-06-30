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
    List<GroupMembership> GroupsBelongingToUserAccountNumberString(string acctno);

    List<GroupMembership> AllGroupMembersByGroupId(int groupid);

  }
}
