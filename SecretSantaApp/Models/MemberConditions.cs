using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public class MemberConditions
  {
    [Key]
    public int ConditionId { get; set; }

    public int GroupId { get; set; }

    public string ConditionalUserAcctNo { get; set; }

    public int ConditionType { get; set; } //Use this later to determine 'CAN HAVE' 'CANT HAVE'

    public virtual void Update(MemberConditions mc)
    {
      ConditionId = mc.ConditionId;
      GroupId = mc.GroupId;
      ConditionalUserAcctNo = mc.ConditionalUserAcctNo;
      ConditionType = mc.ConditionType;
    }
  }
}
