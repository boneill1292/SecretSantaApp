using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public interface IGroupMemberDetailDal
  {

    GroupMemberDetails SaveMemberToGroup(GroupMemberDetails g);
    List<GroupMemberDetails> GroupsBelongingToUserAccountNumberString(string acctno);

  }
}
