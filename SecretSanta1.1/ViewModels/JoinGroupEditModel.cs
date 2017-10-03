using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class JoinGroupEditModel
    {
        public CustomUserEditModel CustomUser { get; set; }
        public List<Group> GroupsNotMemberOf { get; set; }

        //group info
        // public Group Group { get; set; }
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        //[Required]
        [Display(Name = "Enter the Group Password")]
        public string UserInputGroupPassword { get; set; }

        public bool Verified { get; set; }

        public string ErrorMsg { get; set; }
    }
}