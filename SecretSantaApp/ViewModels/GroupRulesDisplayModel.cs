using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class GroupRulesDisplayModel
    {
        public bool Saved { get; set; }

        public string GroupName { get; set; }

        public int GroupId { get; set; }
        public List<GroupRules> GroupRules { get; set; }

        public List<MemberConditions> GroupConditions { get; set; }
    }
}