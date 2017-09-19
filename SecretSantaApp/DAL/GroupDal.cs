using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApp.Models
{
    public class GroupDal : IGroupDal
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public Group GetGroupById(int groupid)
        {
            return _appDbContext.Groups.FirstOrDefault(g => g.GroupId == groupid);
        }


        public List<Group> AllActiveGroups()
        {
            return _appDbContext.Groups.Where(x => x.Active).ToList();
        }

        public List<Group> GroupByGroupName(string groupname)
        {
            return _appDbContext.Groups.Where(x => x.GroupName == groupname).ToList();
        }


        public Group SaveNewGroup(Group g)
        {
            var result = new Group();
            result.GroupName = g.GroupName;
            result.GroupPassWord = g.GroupPassWord;
            result.InsertedBy = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            result.Active = true;
            result.InsertedDateTime = DateTime.Now;

            _appDbContext.Groups.Add(result);
            _appDbContext.SaveChanges();

            return result;
        }

        public Group SaveGroup(Group g)
        {
            if (g.GroupId >= 1)
            {
              //  _appDbContext.Add(g);
                _appDbContext.Update(g);
                _appDbContext.SaveChanges();
                return g;
            }
            var result = new Group();
            result.Update(g);
            _appDbContext.Add(result);
            _appDbContext.SaveChanges();
            return result;
        }
    }
}