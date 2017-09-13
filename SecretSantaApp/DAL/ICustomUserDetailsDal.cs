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