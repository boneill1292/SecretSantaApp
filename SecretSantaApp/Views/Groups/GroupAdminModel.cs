using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.Views.Groups
{
  public class GroupAdminModel
  {
    public CustomUser LoggedInuser { get; set; }
    public List<Group> ActiveGroups { get; set; }
    public List<Group> UnjoinedGroups { get; set; }
  }
}
