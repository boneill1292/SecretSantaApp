using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class CustomUserDetailsDisplayModel : CustomUserDetails
    {
        public bool Saved { get; set; }
        public string UserFullName { get; set; }
    }
}