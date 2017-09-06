using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public class GroupPairingsDal : IGroupPairingsDal
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupPairingsDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public GroupPairings Save(GroupPairings m)
        {
            if (m.ID >= 1)
            {
                _appDbContext.Add(m);
                _appDbContext.Update(m);
                _appDbContext.SaveChanges();
                return m;
            }
            var result = new GroupPairings();
            result.Update(m);


            _appDbContext.GroupPairings.Add(result);
            _appDbContext.SaveChanges();

            return result;
        }

        public List<GroupPairings> GroupPairingsByGroupId(int groupid)
        {
            // var result = new GroupPairings();
            var result = _appDbContext.GroupPairings.Where(u => u.GroupId == groupid).ToList();


            //_appDbContext.Groups.Where(x => x.Active).ToList();`
            return result;
        }

        public GroupPairings Delete(GroupPairings m)
        {
           // var result = new GroupPairings();
            //result.Update(m);

            _appDbContext.Remove(m);
            _appDbContext.SaveChanges();
            return m;
        }



    }
}