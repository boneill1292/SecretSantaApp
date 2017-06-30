using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApp.Models
{
  public class GroupMemberDetailDal : IGroupMemberDetailDal
  {

    private readonly AppDbContext _appDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GroupMemberDetailDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
    {
      _appDbContext = appDbContext;
      _httpContextAccessor = httpContextAccessor;
    }

    public GroupMemberDetails SaveMemberToGroup(GroupMemberDetails g)
    {
      var result = new GroupMemberDetails();
      result.Update(g);
      _appDbContext.Add(result);

      _appDbContext.SaveChanges();

      return result;

    }


    public List<GroupMemberDetails> GroupsBelongingToUserAccountNumberString(string acctno)
    {
      var result = new List<GroupMemberDetails>();

      result = _appDbContext.GroupMemberDetails.Where(x => x.AccountNumberString == acctno).ToList();
      //_appDbContext.Groups.Where(x => x.Active).ToList();

      return result;
    }
  }
}
