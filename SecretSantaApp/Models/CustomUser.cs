using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public class CustomUser
  {
    [Key]
    public int UserId { get; set; }
    public string Email { get; set; }
    public string AccountNumberString { get; set; } //* unique assigned by auth0
    public string FullName { get; set; }

   
  }
}
