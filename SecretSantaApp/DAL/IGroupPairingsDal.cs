using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public interface IGroupPairingsDal
    {
        //CustomUserDetails UserDetailsByCustomUserAcctNo(string acctno);

        // CustomUserDetails UserDetailsByUserId(int userid);
        GroupPairings Save(GroupPairings m);

        List<GroupPairings> GroupPairingsByGroupId(int groupid);

        GroupPairings Delete(GroupPairings m);

        // CustomUserDetails Delete(CustomUserDetails m);
    }
}
