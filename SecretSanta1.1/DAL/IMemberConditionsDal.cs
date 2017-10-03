using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public interface IMemberConditionsDal
    {
        MemberConditions MemberConditionByConditionId(int id);

        List<MemberConditions> MemberConditionsByGroupId(int groupid);

        MemberConditions Save(MemberConditions m);

        MemberConditions Delete(MemberConditions m);

        List<MemberConditions> MemberConditionsByGroupIdByAcctNo(int groupid, string acctno);
    }
}