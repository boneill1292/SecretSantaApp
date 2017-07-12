using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
  public class GroupRulesDal : IGroupRulesDal
  {

    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupRulesDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
      _appDbContext = appDbContext;
      _httpContextAccessor = httpContextAccessor;
    }

  
    public GroupRules SaveRules(GroupRules g)
    {
      var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUser>("LoggedInUser");

      if (g.ID >= 1)
      {
        _appDbContext.Add(g);
        _appDbContext.Update(g);
        _appDbContext.SaveChanges();
        return g;
      }
      else
      {
        var result = new GroupRules();
        result.Update(g);
        result.InsertedBy = u.AccountNumberString;
        //result.AccountNumberString = u.AccountNumberString;
        _appDbContext.Add(result);
        _appDbContext.SaveChanges();
        return result;
      }
    }

    public GroupRules DeleteRule(GroupRules g)
    {
      var result = new GroupRules();
      result.Update(g);

      //_appDbContext.Add(result);
      _appDbContext.Remove(result);
      _appDbContext.SaveChanges();
      return result;
    }

    public List<GroupRules> RulesByGroupId(int groupid)
    {
      var result = new List<GroupRules>();
      result = _appDbContext.GroupRules.Where(x => x.GroupId == groupid).ToList();
      return result;
    }


    public GroupRules GetRuleByRuleId(int ruleid)
    {
      return _appDbContext.GroupRules.FirstOrDefault(g => g.ID == ruleid);
    }
  }
}
