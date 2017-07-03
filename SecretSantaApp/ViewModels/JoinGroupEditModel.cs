using System;
using System.Collections.Generic;
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
    }
}
