using System.Collections.Generic;

namespace SecretSantaApp.Models
{
    public interface ICustomUserDal
    {
        List<CustomUser> AllUsers();
        CustomUser CustomUserByUserId(int id);
        CustomUser CustomUserByAccountNumber(string acctno);


        CustomUser SaveUser(CustomUser u);

        //CustomUser UpdateCustomUserImage(CustomUser u);
    }
}