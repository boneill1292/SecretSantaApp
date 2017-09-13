using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public interface IGroupRulesDal
    {
        GroupRules SaveRules(GroupRules g);

        GroupRules DeleteRule(GroupRules g);
        List<GroupRules> RulesByGroupId(int groupid);

        GroupRules GetRuleByRuleId(int ruleid);
    }
}