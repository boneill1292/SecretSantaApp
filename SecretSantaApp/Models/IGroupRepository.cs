using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public interface IGroupRepository
  {
    //IEnumerable<Group> Groups { get; }
    // Pie GetPieById(int pieId);
    Group GetGroupById(int groupid);
  }
}