using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.ViewModels
{
    public class LoginViewModel
    {
    [Required]
      [EmailAddress]
      [Display(Name = "Email Address")]
      public string EmailAddress { get; set; }

      [Required]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string Password { get; set; }
  }
}
