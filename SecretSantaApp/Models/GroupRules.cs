using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
