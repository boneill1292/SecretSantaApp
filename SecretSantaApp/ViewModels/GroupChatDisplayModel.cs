using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class GroupChatDisplayModel
    {
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public List<GroupMessages> MessagesList { get; set; }
    }
}