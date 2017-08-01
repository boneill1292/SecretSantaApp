using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSantaApp.Models;

namespace SecretSantaApp.ViewModels
{
    public class CustomUserDetailsDisplayModel : CustomUserDetails
    {
        public bool Saved { get; set; }
        public string UserFullName { get; set; }
    }
}
