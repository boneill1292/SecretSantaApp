using System.Collections.Generic;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public interface IGroupMessagesDal
    {
        GroupMessages Save(GroupMessages g);

        GroupMessages Delete(GroupMessages g);
        List<GroupMessages> MessagesByGroupId(int groupid);

        GroupMessages MessageByMessageId(int messageid);
    }
}