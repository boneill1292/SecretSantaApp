using System.Collections.Generic;

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