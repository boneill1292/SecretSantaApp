using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public interface ICustomUserRepository
  {
    List<CustomUser> AllUsers();
    CustomUser CustomUserByUserId(int id);
    bool CustomUserByAccountNumber(string acctno);


    CustomUser SaveUser(CustomUser u);
  }
}
