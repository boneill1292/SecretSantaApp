using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
  public interface IGroupRulesDal
  {

    GroupRules SaveRules(GroupRules g);

    List<GroupRules> RulesByGroupId(int groupid);

    GroupRules GetRuleByRuleId(int ruleid);
  }
}
