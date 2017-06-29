using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public class Group
  {
    public int GroupId { get; set; }
    public string GroupName { get; set; }
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
