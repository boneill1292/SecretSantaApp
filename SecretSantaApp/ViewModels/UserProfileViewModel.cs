using System.Collections;
using System.Collections.Generic;

namespace SecretSantaApp.ViewModels
{
    public class UserProfileViewModel
    {
        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string ProfileImage { get; set; }
        public string UserAcctNo { get; set; }
        public int UserId { get; set; }

        public virtual IEnumerable Claims { get; set; }
    }
}