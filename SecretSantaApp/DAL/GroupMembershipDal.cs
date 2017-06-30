using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApp.Models
{
  public class GroupMembershipDal : IGroupMembershipDal
  {

    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupMembershipDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
      _appDbContext = appDbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public GroupMembership SaveMemberToGroup(GroupMembership g)
    {
      var result = new GroupMembership();
      result.Update(g);
      _appDbContext.Add(result);

      _appDbContext.SaveChanges();

      return result;

    }


    public List<GroupMembership> GroupsBelongingToUserAccountNumberString(string acctno)
    {
      var result = new List<GroupMembership>();

      result = _appDbContext.GroupMembership.Where(x => x.AccountNumberString == acctno).ToList();
      //_appDbContext.Groups.Where(x => x.Active).ToList();

      return result;
    }


    //public List<GroupMembership> GroupsUserDoesNotBelongToByAccountNumberString(string acctno)
    //{
    //  var result = new List<GroupMembership>();

    //  result = _appDbContext.GroupMembership.Where(x => x.AccountNumberString != acctno).ToList()

    //    return result;
    //}

  }
}
