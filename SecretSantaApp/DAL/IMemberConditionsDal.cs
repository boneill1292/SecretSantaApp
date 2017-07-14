using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
  public interface IMemberConditionsDal
  {
    MemberConditions MemberConditionByConditionId(int id);

    List<MemberConditions> MemberConditionsByGroupId(int groupid);

    MemberConditions Save(MemberConditions m);

    MemberConditions Delete(MemberConditions m);
  }
}
