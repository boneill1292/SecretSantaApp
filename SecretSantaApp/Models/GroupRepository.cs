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


    public List<Group> AllActiveGroups()
    {
      return _appDbContext.Groups.Where(x => x.Active).ToList();
    }
    //public IEnumerable<Pie> Pies
    //{
    //  get
    //  {
    //    return _appDbContext.Pies.Include(c => c.Category);
    //  }
    //}


    public void CreateGroup(Group g)
    {
      var group = new Group();
      group.GroupName = g.GroupName;

      _appDbContext.Groups.Add(group);
      _appDbContext.SaveChanges();
    }







  }

}

