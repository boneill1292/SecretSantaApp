using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class Group
    {
        //[Display(ResourceType = typeof(SiteResource), Name = "AlarmExpFailureNoteName",
        //  Description = "AlarmExpFailureNoteDescription")]
        //[StringLength(250, ErrorMessageResourceType = typeof(SiteResource), ErrorMessage = "",
        //  ErrorMessageResourceName = "AlarmExpFailureNoteTooLong")]
        public int GroupId { get; set; }

        [DisplayName("Group Name")]
        [Required(ErrorMessage = "The Name is Required!")]
        public string GroupName { get; set; }

        [DisplayName("Group Pass Word")]
        [Required(ErrorMessage = "The Password is Required!")]
        public string GroupPassWord { get; set; }

        public bool Active { get; set; }

        public string InsertedBy { get; set; }
        public DateTime InsertedDateTime { get; set; }


        public virtual void Update(Group g)
        {
            GroupId = g.GroupId;
            GroupName = g.GroupName;
            GroupPassWord = g.GroupPassWord;
            Active = g.Active;
            InsertedBy = g.InsertedBy;
            InsertedDateTime = g.InsertedDateTime;
        }
    }
}