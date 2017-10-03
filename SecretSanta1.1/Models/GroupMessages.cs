using System;
using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class GroupMessages
    {
        [Key]
        public int ID { get; set; }

        public int GroupId { get; set; }

        public string Message { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDtm { get; set; }

        public virtual void Update(GroupMessages m)
        {
            ID = m.ID;
            GroupId = m.GroupId;
            Message = m.Message;
            InsertedBy = m.InsertedBy;
            InsertedDtm = m.InsertedDtm;
        }
    }
}