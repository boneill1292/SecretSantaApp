using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
    public class GroupPairings
    {
        [Key]
        public int ID { get; set; }

        public int GroupId { get; set; }

        public string PersonOne { get; set; }

        public string PersonTwo { get; set; }

        public virtual void Update(GroupPairings m)
        {
            ID = m.ID;
            GroupId = m.GroupId;
            PersonOne = m.PersonOne;
            PersonTwo = m.PersonTwo;
        }
    }
}
