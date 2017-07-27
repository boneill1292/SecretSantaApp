using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public interface ICustomUserDetailsDal
    {
        CustomUserDetails UserDetailsByCustomUserAcctNo(string acctno);

        CustomUserDetails UserDetailsByUserId(int userid);
        CustomUserDetails Save(CustomUserDetails m);

        CustomUserDetails Delete(CustomUserDetails m);


    }
}
