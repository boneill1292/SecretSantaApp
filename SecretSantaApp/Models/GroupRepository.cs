using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSantaApp.Models
{
  public class GroupRepository : IGroupRepository
  {
    private readonly AppDbContext _appDbContext;

    public GroupRepository(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }


    public Group GetGroupById(int groupid)
    {
      return _appDbContext.Groups.FirstOrDefault(g => g.GroupId == groupid);
    }
  }
  //private readonly AppDbContext _appDbContext;

  //   return _appDbContext.Pies.FirstOrDefault(p => p.PieId == pieId);
  //  public PieRepository(AppDbContext appDbContext)
  //  {
  //    _appDbContext = appDbContext;
  //  }

  //  public IEnumerable<Pie> Pies
  //  {
  //    get
  //    {
  //      return _appDbContext.Pies.Include(c => c.Category);
  //    }
  //  }
}

