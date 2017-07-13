using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.BL;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public class GroupMessagesDal : IGroupMessagesDal
    {

      private readonly AppDbContext _appDbContext;
      private readonly IHttpContextAccessor _httpContextAccessor;

      public GroupMessagesDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
      {
        _appDbContext = appDbContext;
        _httpContextAccessor = httpContextAccessor;
      }



    public GroupMessages Save(GroupMessages g)
    {
     
      //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");
      //var liu = _httpContextAccessor.HttpContext.User;
     // var u = _secretSantaBl.CustomUserModelByLoggedInUser(liu);

      if (g.ID >= 1)
      {
        _appDbContext.Add(g);
        _appDbContext.Update(g);
        _appDbContext.SaveChanges();
        return g;
      }
      else
      {
        var result = new GroupMessages();
        result.Update(g);
        //result.InsertedBy = u.AccountNumberString;
        _appDbContext.Add(result);
        _appDbContext.SaveChanges();
        return result;
      }
    }

    public GroupMessages Delete(GroupMessages g)
    {
      var result = new GroupMessages();
      result.Update(g);

      //_appDbContext.Add(result);
      _appDbContext.Remove(result);
      _appDbContext.SaveChanges();
      return result;
    }

    public List<GroupMessages> MessagesByGroupId(int groupid)
    {
      var result = new List<GroupMessages>();
      result = _appDbContext.GroupMessages.Where(x => x.GroupId == groupid).ToList();
      return result;
    }


    public GroupMessages MessageByMessageId(int messageid)
    {
      return _appDbContext.GroupMessages.FirstOrDefault(g => g.ID == messageid);
    }
  }
}
