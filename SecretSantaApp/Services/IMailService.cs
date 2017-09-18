using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Services
{
    public interface IMailService
    {
         Task<bool> SendAsync();
    }
}
