using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class GroupRules
    {
        [Key]
        public int ID { get; set; }

        public int GroupId { get; set; }

        public string Rule { get; set; }

        public string InsertedBy { get; set; }

        public virtual void Update(GroupRules m)
        {
            ID = m.ID;
            GroupId = m.GroupId;
            Rule = m.Rule;
            InsertedBy = m.InsertedBy;
        }
    }
}