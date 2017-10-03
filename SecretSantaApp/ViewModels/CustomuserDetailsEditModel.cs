using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class CustomUserDetailsEditModel : CustomUserDetails
    {
        public bool Saved { get; set; }

        public bool IsMe { get; set; }

        public string UserFullName { get; set; }
    }
}