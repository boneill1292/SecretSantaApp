﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using SecretSantaApp.Models;

namespace SecretSantaApp.DAL
{
    public class MemberConditionsDal : IMemberConditionsDal
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberConditionsDal(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public MemberConditions MemberConditionByConditionId(int id)
        {
            return _appDbContext.MemberConditions.FirstOrDefault(g => g.ConditionId == id);
        }

        public List<MemberConditions> MemberConditionsByGroupId(int groupid)
        {
            var result = new List<MemberConditions>();
            result = _appDbContext.MemberConditions.Where(x => x.GroupId == groupid).ToList();
            return result;
        }


        public List<MemberConditions> MemberConditionsByGroupIdByAcctNo(int groupid, string acctno)
        {
            var result = new List<MemberConditions>();
            result = _appDbContext.MemberConditions
                .Where(x => x.GroupId == groupid && x.UserReceivingConditionAcctNo == acctno).ToList();
            return result;
        }


        public MemberConditions Save(MemberConditions m)
        {
            if (m.ConditionId >= 1)
            {
                _appDbContext.Update(m);
                _appDbContext.Add(m);
                _appDbContext.SaveChanges();
                return m;
            }
            var result = new MemberConditions();
            result.Update(m);
            _appDbContext.Add(result);
            _appDbContext.SaveChanges();
            return result;
        }

        public MemberConditions Delete(MemberConditions m)
        {
            var result = new MemberConditions();
            result.Update(m);

            _appDbContext.Remove(result);
            _appDbContext.SaveChanges();
            return result;
        }
    }
}