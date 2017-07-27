using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public class CustomUserDetailsDal : ICustomUserDetailsDal


    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomUserDetailsDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public CustomUserDetails UserDetailsByCustomUserAcctNo(string acctno)
        {
            return _appDbContext.CustomUserDetails.FirstOrDefault(g => g.UserAcctNo == acctno);
        }

        public CustomUserDetails UserDetailsByUserId(int userid)
        {
            return _appDbContext.CustomUserDetails.FirstOrDefault(g => g.UserId == userid);
        }


        public CustomUserDetails Save(CustomUserDetails d)
        {
            if (d.DetailsId >= 1)
            {
                _appDbContext.Update(d);
               // _appDbContext.Add(d);
                _appDbContext.SaveChanges();
                return d;
            }
            else
            {
                var result = new CustomUserDetails();
                result.Update(d);
                _appDbContext.Add(result);
                _appDbContext.SaveChanges();
                return result;
            }
        }

        public CustomUserDetails Delete(CustomUserDetails m)
        {
            throw new NotImplementedException();
        }
    }
}
