using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
  public class MemberConditionsEditModel : MemberConditions
  {
    public string GroupName { get; set; }

    public string UserReceivingConditionName { get; set; }
    
    public string SelectedUserName { get; set; }

    public int MembershipId { get; set; }

    public bool Saved { get; set; }

    public List<GroupMembership> OtherGroupMembers { get; set; }
  }
}
