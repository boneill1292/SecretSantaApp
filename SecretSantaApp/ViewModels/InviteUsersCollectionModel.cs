using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.ViewModels
{
  public class InviteUsersCollectionModel
  {
    public List<InviteUsersViewModel> UsersToInvite { get; set; }
    public int InviteMethod { get; set; }
    
    //Thinking of using the invite method to determine if we should send emails or texts
  }
}
