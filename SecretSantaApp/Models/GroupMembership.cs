using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class GroupMembership
    {
        [Key]
        public int ID { get; set; }

        public int GroupId { get; set; }

        public string AccountNumberString { get; set; }


        public virtual void Update(GroupMembership m)
        {
            ID = m.ID;
            GroupId = m.GroupId;
            AccountNumberString = m.AccountNumberString;
        }
    }
}