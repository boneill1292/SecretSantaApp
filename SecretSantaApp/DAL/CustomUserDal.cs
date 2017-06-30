using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApp.Models
{
  public class CustomUserDal : ICustomUserDal
  {
    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomUserDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
      _appDbContext = appDbContext;
      _httpContextAccessor = httpContextAccessor;
    }
    public List<CustomUser> AllUsers()
    {
      throw new NotImplementedException();
    }


    public CustomUser CustomUserByUserId(int id)
    {
      var result = new CustomUser();
      result = _appDbContext.CustomUsers.FirstOrDefault(u => u.UserId == id);
      return result;
      
    }

    public CustomUser CustomUserByAccountNumber(string acctno)
    {
      var result = new CustomUser();
      result = _appDbContext.CustomUsers.FirstOrDefault(u => u.AccountNumberString == acctno);
   
      return result;
    }



    public CustomUser SaveUser(CustomUser u)
    {
      var result = new CustomUser();
      result.Update(u);
      
      //result.Email = u.Email;
      //result.FullName = u.FullName;
      //result.AccountNumberString = u.AccountNumberString;

      _appDbContext.CustomUsers.Add(result);
      _appDbContext.SaveChanges();

      return result;
      //_appDbContext.Groups.Add(result);
      //_appDbContext.SaveChanges();
    }

  }
}
