using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public interface IGroupDal
  {

    Group GetGroupById(int groupid);

    List<Group> AllActiveGroups();

    List<Group> GroupByGroupName(string groupname);

    Group SaveNewGroup(Group g);
  }
}