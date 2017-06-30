using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
