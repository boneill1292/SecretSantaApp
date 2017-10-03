using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class GroupRulesEditModel : GroupRules
    {
        public bool Saved { get; set; }

        public string GroupName { get; set; }
        //public int GroupId { get; set; }
    }
}