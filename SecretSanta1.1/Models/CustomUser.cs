using System.ComponentModel.DataAnnotations;

namespace SecretSantaApp.Models
{
    public class CustomUser
    {
        [Key]
        public int UserId { get; set; }

        public string Email { get; set; }
        public string AccountNumberString { get; set; } //* unique assigned by auth0
        public string FullName { get; set; }
        public string ProfileImage { get; set; }


        public virtual void Update(CustomUser cu)
        {
            UserId = cu.UserId;
            Email = cu.Email;
            AccountNumberString = cu.AccountNumberString;
            FullName = cu.FullName;
            ProfileImage = cu.ProfileImage;
        }
    }
}