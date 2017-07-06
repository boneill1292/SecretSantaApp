using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class JoinGroupEditModel
    {
      public CustomUser CustomUser { get; set; }
      public List<Group> GroupsNotMemberOf { get; set; }
      
      //group info
      public Group Group { get; set; }
      public int GroupId { get; set; }
      
      [Required]
      [Display(Name ="Enter the Group Password")]
      public string UserInputGroupPassword { get; set; }
      
    }
}
