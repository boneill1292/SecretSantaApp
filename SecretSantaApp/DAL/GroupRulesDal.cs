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

      var result = new GroupRules();
      result.Update(g);
      result.InsertedBy = u.FullName;
      //result.AccountNumberString = u.AccountNumberString;
      _appDbContext.Add(result);

      _appDbContext.SaveChanges();

      return result;
    }

    public List<GroupRules> RulesByGroupId(int groupid)
    {
      throw new NotImplementedException();
    }
  }
}
