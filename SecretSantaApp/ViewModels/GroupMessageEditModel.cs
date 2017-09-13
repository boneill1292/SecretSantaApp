using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class GroupMessageEditModel : GroupMessages
    {
        public bool Saved { get; set; }
        public string GroupName { get; set; }
    }
}