using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public interface ICustomUserDal
  {
    List<CustomUser> AllUsers();
    CustomUser CustomUserByUserId(int id);
    CustomUser CustomUserByAccountNumber(string acctno);


    CustomUser SaveUser(CustomUser u);
  }
}
