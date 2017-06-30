using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
  public class GroupHomeEditModel : Group

  {
    public bool Saved { get; set; }
    public List<CustomUser> GroupMembers { get; set; }
  }
}
