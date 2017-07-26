using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecretSantaApp.DAL;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.BL
{
    public class SecretSantaBl : ISecretSantaBl
    {

        private readonly IGroupDal _groupDal;
        private readonly ICustomUserDal _customUserDal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGroupMembershipDal _groupMembershipDal;
        private readonly IGroupRulesDal _groupRulesDal;
        private readonly IGroupMessagesDal _groupMessagesDal;
        private readonly IMemberConditionsDal _memberConditionsDal;
        private static Random rnd = new Random();
        public SecretSantaBl(IGroupDal groupDal,
                             ICustomUserDal customUserDal,
                             IHttpContextAccessor httpContextAccessor,
                              IGroupMembershipDal groupMembershipDal,
                              IGroupRulesDal groupRulesDal,
                              IGroupMessagesDal groupMessagesDal,
                              IMemberConditionsDal memberConditionsDal)
        {
            _groupDal = groupDal;
            _customUserDal = customUserDal;
            _httpContextAccessor = httpContextAccessor;
            _groupMembershipDal = groupMembershipDal;
            _groupRulesDal = groupRulesDal;
            _groupMessagesDal = groupMessagesDal;
            _memberConditionsDal = memberConditionsDal;
        }

        public CustomUserEditModel CustomUserModelByLoggedInUser(ClaimsPrincipal user)
        {
            var acctid = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var existinguser = _customUserDal.CustomUserByAccountNumber(acctid);

            if (existinguser != null)
            {
                var existingusereditmodel = new CustomUserEditModel();
                existingusereditmodel.Update(existinguser);
                existingusereditmodel.NewUser = false;
                return existingusereditmodel;
            }
            else
            {
                var result = new CustomUserEditModel();
                var name = user.Identity.Name;
                var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                var pic = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

                result.AccountNumberString = acctid;
                result.FullName = name;
                result.Email = email;
                result.ProfileImage = pic;
                result.NewUser = true;
                result.UserId = 0;
                return result;
            }


        }



        public string UserFullNameByAccountNumberString(string acctno)
        {
            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            var result = user.FullName;

            return result;
        }

        public string UserImageByAccountNumberString(string acctno)
        {
            var u = _customUserDal.CustomUserByAccountNumber(acctno);

            var result = u.ProfileImage;

            return result;
        }

        public GroupAdminModel DefaultGroupAdminModel()
        {
            var result = new GroupAdminModel();
            result.ActiveGroups = _groupDal.AllActiveGroups();

            return result;
        }


        public GroupEditModel DefaultGroupEditModel()
        {
            var result = new GroupEditModel();
            return result;
        }

        public GroupEditModel SaveNewGroup(GroupEditModel model)
        {
            //Check to make sure there are values
            if (model.GroupName == null)
            {
                throw new Exception("Name is Required");
            }

            if (model.GroupName == "abc123")
            {
                throw new Exception("You are dumb");
            }

            //code to get the currently logged in user
            var liu = _httpContextAccessor.HttpContext.User;
            var u = CustomUserModelByLoggedInUser(liu);

            //Save the new group
            var saved = _groupDal.SaveNewGroup(model);
            model.Update(saved);

            //Now we will add the user who is creating the group to the new group
            var gmd = new GroupMembershipEditModel();

            //gmd.AccountNumberString = "";
            gmd.AccountNumberString = u.AccountNumberString;
            gmd.GroupId = saved.GroupId;

            _groupMembershipDal.SaveMemberToGroup(gmd);

            model.Saved = true;
            return model;
        }


        public CustomUserEditModel CheckUserByCustomUserAccountNumber(CustomUserEditModel model)
        {
            //First check to see if this user exists
            //_customUserDal.SaveUser(model);
            //var exists = _customUserDal.CustomUserByAccountNumber(model.AccountNumberString);

            if (model.NewUser)
            {
                var m = new CustomUser();
                m.Update(model);

                var saved = _customUserDal.SaveUser(m);

                model.Update(saved);
                return model;
            }
            else
            {
                return model;
            }


        }


        public void JoinGroupAsCustomUser(CustomUserEditModel user, int groupid)
        {

            var gmd = new GroupMembershipEditModel();
            gmd.AccountNumberString = user.AccountNumberString;
            gmd.GroupId = groupid;

            _groupMembershipDal.SaveMemberToGroup(gmd);

        }


        public MyGroupsViewModel MyGroupsViewModelByUserId(CustomUserEditModel user)
        {
            var result = new MyGroupsViewModel();
            result.MyGroups = new List<Group>();
            var grouplist = new List<Group>();

            var groupsibelongto = _groupMembershipDal.GroupsBelongingToUserAccountNumberString(user.AccountNumberString);

            foreach (var g in groupsibelongto)
            {
                var group = new Group();
                group = _groupDal.GetGroupById(g.GroupId);
                grouplist.Add(group);
            }

            result.MyGroups = grouplist;

            return result;
        }


        public GroupHomeEditModel GroupHomeEditModelByGroupId(int groupid)
        {
            var result = new GroupHomeEditModel();
            var userlist = new List<CustomUserEditModel>();

            var liu = _httpContextAccessor.HttpContext.User;
            var loggedinuser = CustomUserModelByLoggedInUser(liu);
            //var loggedinuser = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");

            var group = _groupDal.GetGroupById(groupid);


            var admin = _customUserDal.CustomUserByAccountNumber(group.InsertedBy);

            result.GroupAdmin = admin;
            //if the user is the person who created the group assign them to admin.
            result.GroupAdminBool = @group.InsertedBy == loggedinuser.AccountNumberString;

            result.Update(group);

            var groupmembership = _groupMembershipDal.AllGroupMembersByGroupId(groupid);

            foreach (var g in groupmembership)
            {
                var user = new CustomUser();
                var cu = new CustomUserEditModel();
                user = _customUserDal.CustomUserByAccountNumber(g.AccountNumberString);
                cu.Update(user);
                userlist.Add(cu);
            }

            //get the group conditions
            //var conditions = _memberConditionsDal.MemberConditionsByGroupId(groupid;


            result.GroupMembers = userlist;
            result.GroupMembershipModelList = groupmembership;
            result.NewGroup = false;
            //result.GroupConditions = conditions;

            return result;
        }




        public JoinGroupEditModel JoinGroupEditModelByAccountNumberString(string acctno)
        {
            var result = new JoinGroupEditModel();
            var groupslist = new List<Group>();
            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            var activegroupsidlist = new List<int>();
            var groupsmemberofidlist = new List<int>();

            var allactivegroups = _groupDal.AllActiveGroups();
            var groupsmemberof = _groupMembershipDal.GroupsBelongingToUserAccountNumberString(user.AccountNumberString);


            foreach (var ag in allactivegroups)
            {
                activegroupsidlist.Add(ag.GroupId);
            }

            foreach (var ag in groupsmemberof)
            {
                groupsmemberofidlist.Add(ag.GroupId);
            }


            var results = activegroupsidlist.Where(m => !groupsmemberofidlist.Contains(m));

            foreach (var r in results)
            {
                var group = new Group();
                group = _groupDal.GetGroupById(r);
                groupslist.Add(group);
            }

            //var matchItem = List1.Intersect(List2).First();



            //foreach (var g in groupsmemberof)
            //{
            //  var group = new Group();
            //  group = _groupDal.GetGroupById(g.GroupId);
            //  groupslist.Add(group);
            //}

            result.CustomUser = new CustomUserEditModel();
            result.CustomUser.Update(user);

            result.GroupsNotMemberOf = groupslist;

            return result;
        }



        public InviteUsersCollectionModel InviteUsersCollectionModelByAmountToGet(int amount)
        {

            var result = new InviteUsersCollectionModel();
            result.UsersToInvite = new List<InviteUsersViewModel>();

            if (amount <= 0)
            {
                return result;
            }
            else
            {
                for (var a = 0; a < amount; a++)
                {
                    var usertoinvite = new InviteUsersViewModel();
                    result.UsersToInvite.Add(usertoinvite);
                }
            }
            return result;
        }


        public InviteUsersViewModel AdditionalInviteUsersViewModel(int tempid)
        {
            var result = new InviteUsersViewModel();
            result.TempId = tempid;
            return result;
        }


        public JoinGroupEditModel JoinGroupEditModelByGroupId(int groupid)
        {
            var group = _groupDal.GetGroupById(groupid);

            if (group == null)
            {
                throw new Exception("Error loading group");
            }



            var result = new JoinGroupEditModel();
           // result.Group = group; //This was causing validation errors... why do i even need this?
            result.GroupName = group.GroupName;
            result.GroupId = groupid;
            result.Verified = false;

            return result;

        }


        public JoinGroupEditModel CheckPasswordInput(JoinGroupEditModel model)
        {
            var group = _groupDal.GetGroupById(model.GroupId);

            if (group == null)
            {
                throw new Exception("Error loading group");
            }

            if (model.UserInputGroupPassword == null)
            {
                throw new Exception("Password is required");
            }

            var password = group.GroupPassWord;

            if (model.UserInputGroupPassword != password)
            {
                throw new Exception("Incorrect password");
                //model.Verified = false;
                //model.ErrorMsg = "Incorrect Password";
                //model.Group = group;
                //model.GroupId = group.GroupId;
                //return model;
            }
            else
            {
                //model.Group = group;
                model.GroupId = group.GroupId;
                model.GroupName = group.GroupName;

                JoinGroupAsCustomUser(model.CustomUser, model.GroupId);
                model.Verified = true;
                model.ErrorMsg = null;
                return model;
            }

        }





        public GroupRulesEditModel NewRuleEditModelByGroupId(int groupid)
        {
            var group = _groupDal.GetGroupById(groupid);

            var result = new GroupRulesEditModel();
            result.GroupName = group.GroupName;
            result.GroupId = groupid;


            return result;
        }



        public GroupRulesEditModel GroupRuleEditModelByRuleId(int ruleid)
        {
            var rule = _groupRulesDal.GetRuleByRuleId(ruleid);
            var group = _groupDal.GetGroupById(rule.GroupId);
            var result = new GroupRulesEditModel();
            result.Update(rule);
            result.GroupName = group.GroupName;

            return result;

        }


        public GroupRulesEditModel SaveGroupRules(GroupRulesEditModel model)
        {
            if (model.Rule == null)
            {
                throw new Exception("Required");
            }

            var m = new GroupRules();
            m.Update(model);

            var liu = _httpContextAccessor.HttpContext.User;
            var u = CustomUserModelByLoggedInUser(liu);

            m.InsertedBy = u.AccountNumberString;

            var saved = _groupRulesDal.SaveRules(m);

            var result = new GroupRulesEditModel();
            result.Update(saved);

            return result;
        }

        public GroupRulesEditModel DeleteGroupRule(GroupRulesEditModel model)
        {
            var m = new GroupRules();
            m.Update(model);

            var deleted = _groupRulesDal.DeleteRule(m);
            var result = new GroupRulesEditModel();
            result.Update(deleted);
            return result;
        }


        public GroupRulesDisplayModel GroupRulesDisplayModelByGroupId(int groupid)
        {
            var group = _groupDal.GetGroupById(groupid);
            var grouprules = _groupRulesDal.RulesByGroupId(groupid);
            var groupconditions = _memberConditionsDal.MemberConditionsByGroupId(groupid);
            //var conditionstringlist = new List<string>();

            //foreach (var c in groupconditions)
            //{
            //    var ur = _customUserDal.CustomUserByAccountNumber(c.UserReceivingConditionAcctNo);
            //    var us = _customUserDal.CustomUserByAccountNumber(c.UserSelectedForConditionAcctNo);

            //    var condition = ur.FullName + " Cannot Have: " + us.FullName;
            //    conditionstringlist.Add(condition);
            //}


            var result = new GroupRulesDisplayModel();
            result.GroupName = group.GroupName;
            result.GroupRules = grouprules;
            result.GroupConditions = groupconditions;
            result.GroupId = groupid;
            return result;
        }


        public GroupChatDisplayModel GroupChatDisplayModelByGroupId(int groupid)
        {
            var group = _groupDal.GetGroupById(groupid);
            var messages = _groupMessagesDal.MessagesByGroupId(groupid);

            var result = new GroupChatDisplayModel();
            result.GroupName = group.GroupName;
            result.GroupId = group.GroupId;
            result.MessagesList = messages;
            return result;
        }


        public GroupMessageEditModel NewGroupMessageEditModelByGroupId(int groupid)
        {
            var result = new GroupMessageEditModel();
            var group = _groupDal.GetGroupById(groupid);

            result.GroupName = group.GroupName;
            return result;

        }

        public GroupMessageEditModel SaveGroupMessage(GroupMessageEditModel model)
        {
            if (model.Message == null)
            {
                throw new Exception("Required");
            }

            var m = new GroupMessages();
            m.Update(model);

            var liu = _httpContextAccessor.HttpContext.User;
            var u = CustomUserModelByLoggedInUser(liu);

            m.InsertedBy = u.AccountNumberString;

            var saved = _groupMessagesDal.Save(m);

            var result = new GroupMessageEditModel();
            result.Update(saved);

            return result;
        }


        public MemberConditionsEditModel MemberConditionsEditModelByMembershipId(int membershipid, string acctno)
        {
            //var userReceivingCondition = _customUserDal.CustomUserByAccountNumber(acctno);
            var members = _groupMembershipDal.GroupMembershipModelByGroupMembershipId(membershipid);
            var group = _groupDal.GetGroupById(members.GroupId);

            var name = UserFullNameByAccountNumberString(members.AccountNumberString);
            var othergroupmembers = _groupMembershipDal.AllGroupMembersByGroupId(group.GroupId);
            othergroupmembers = othergroupmembers.Where(x => x.AccountNumberString != members.AccountNumberString).ToList();


            var result = new MemberConditionsEditModel();
            result.GroupName = group.GroupName;
            result.UserReceivingConditionName = name;
            result.MembershipId = members.ID;
            result.OtherGroupMembers = othergroupmembers;
            //End the viewmodel fields - need to populate the true model

            result.UserReceivingConditionAcctNo = acctno;
            result.UserSelectedForConditionAcctNo = "";
            result.GroupId = group.GroupId;
            result.ConditionId = 0;
            return result;
        }


        public MemberConditionsEditModel SaveNewMemberCondition(MemberConditionsEditModel model)
        {
            if (model.UserSelectedForConditionMembershipNo == null)
            {
                throw new Exception("Must select a person");
            }

            var user = GetLoggedInUser();

            var membermodel =
              _groupMembershipDal.GroupMembershipModelByGroupMembershipId(model.UserSelectedForConditionMembershipNo);


            //var selectedperson = _customUserDal.CustomUserByAccountNumber(membermodel.AccountNumberString);

            model.UserSelectedForConditionAcctNo = membermodel.AccountNumberString;

            var condition = new MemberConditions();
            condition.Update(model);
            condition.InsertedBy = user.AccountNumberString;
            var saved = _memberConditionsDal.Save(condition);


            //pass the user who is receiving the condition

            //get the user from the selected person

            //save the type of condition

            return model;
        }



        public MemberConditionsEditModel MemberConditionsEditModelByConditionId(int conditionid)
        {
            var condition = _memberConditionsDal.MemberConditionByConditionId(conditionid);
            //var deleted = _memberConditionsDal.Delete(condition);

            var result = new MemberConditionsEditModel();
            result.Update(condition);
           // result.Update(deleted);
            return result;
        }

        public MemberConditionsEditModel DeleteMemberCondition(MemberConditionsEditModel model)
        {
            var condition = _memberConditionsDal.MemberConditionByConditionId(model.ConditionId);
            var deleted = _memberConditionsDal.Delete(condition);

            var result = new MemberConditionsEditModel();
            //result.Update(condition);
             result.Update(deleted);
            return result;
        }


        public SelectList OtherUsersDropDown(string acctnostr, int groupid)
        {
            // var result = new GroupConditionsOtherUsersModel();
            var allgroupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);
            var othergroupmembers = allgroupmembers.Where(x => x.AccountNumberString != acctnostr).ToList();
            var rsltlist = new List<GroupConditionsOtherUsersModel>();

            foreach (var o in othergroupmembers)
            {
                var name = UserFullNameByAccountNumberString(o.AccountNumberString);
                var m = new GroupConditionsOtherUsersModel();
                m.AccountNumberString = o.AccountNumberString;
                m.FullName = name;
                m.MembershipId = o.ID;
                rsltlist.Add(m);
            }

            var result = new SelectList(rsltlist, nameof(GroupConditionsOtherUsersModel.MembershipId),
           nameof(GroupConditionsOtherUsersModel.FullName));


            // var result = new SelectList(othergroupmembers, nameof(GroupMembership.ID),
            //nameof(GroupMembership.AccountNumberString));

            //allgroupmembers = allgroupmembers.Where(x => x.AccountNumberString != members.AccountNumberString).ToList();


            return result;
        }



        public DrawNamesDisplayModel DrawNamesDisplayModelByGroupId(int groupid)
        {
            var result = new DrawNamesDisplayModel();
            var group = _groupDal.GetGroupById(groupid);

            result.Group = group;
            return result;
        }


        public DrawNamesDisplayModel DrawNames(DrawNamesDisplayModel model)
        {

            //Get the logged in users ID
            //var loggedinuser = GetLoggedInUser();
            //var userid = loggedinuser.AccountNumberString;


            var drawnnamelist = new List<DrawNamesEditModel>();
            ////Load all the members in a group
            //var groupmembers = _groupMembershipDal.AllGroupMembersByGroupId(model.Group.GroupId);

            ////Get a list of all members acct no's
            //var allGroupMembersAcctNoList = groupmembers.Select(x => x.AccountNumberString).ToList();

            ////if this works cant copy the list 
            //var tempAllGroupMembersList = groupmembers.Select(x => x.AccountNumberString).ToList();

            ////var randopersonint = rnd.Next(groupmemberacctnolist.Count);
            ////var randoperson = groupmemberacctnolist[randopersonint];

            //foreach (var g in allGroupMembersAcctNoList)
            //{
            //    //GOTO a BL method to generate a random person. 
            //    var d = new DrawNamesEditModel();


            //    //var anyconditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(model.Group.GroupId, g);
            //    //var conditionsList = new List<string>();

            //    //if (anyconditions.Any())
            //    //{
            //    //    foreach (var c in anyconditions)
            //    //    {
            //    //        var u = c.UserSelectedForConditionAcctNo;
            //    //        conditionsList.Add(u);
            //    //    }
            //    //}

            //   // tempAllGroupMembersList.RemoveRange(conditionsList);

            //    //**Check this out
            //    //var resultlist = tempAllGroupMembersList.Except(conditionsList).ToList();

            //    var userone = g;

            //    var usersnotmelist = tempAllGroupMembersList.Where(x => x != g).ToList();

            //    var randopersonint = rnd.Next(usersnotmelist.Count);

            //    var usertwo = tempAllGroupMembersList[randopersonint];

            //    // cant remove from a list so need to make it an empty string?
            //    //allGroupMembersAcctNoList[randopersonint] = string.Empty;

            //    tempAllGroupMembersList.RemoveAt(randopersonint);

            //    d.UserOne = userone;
            //    d.UserTwo = usertwo;
            //    drawnnamelist.Add(d);

            //}



            //Load all of the conditions in the group

            //Cross reference all the members with all the conditions


            drawnnamelist = GetRandomUsersForDrawingNames(model.Group.GroupId);

            model.DrawNamesList = new List<DrawNamesEditModel>();
            model.DrawNamesList = drawnnamelist;
            return model;
        }





        //Helpers
        public CustomUserEditModel GetLoggedInUser()
        {
            var liu = _httpContextAccessor.HttpContext.User;
            var u = CustomUserModelByLoggedInUser(liu);
            return u;
        }

        public string ConditionDisplayByAccountNumbers(string ur, string us)
        {
            var receiving = _customUserDal.CustomUserByAccountNumber(ur);
            var selected = _customUserDal.CustomUserByAccountNumber(us);

            var condition = receiving.FullName + " Cannot Have: " + selected.FullName;
            return condition;
            //conditionstringlist.Add(condition);
        }


        public List<DrawNamesEditModel> GetRandomUsersForDrawingNames(int groupid)
        {
            var drawnnamelist = new List<DrawNamesEditModel>();
            bool redraw = false;

            //Load all the members in a group
            var groupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);

            //Get a list of all members acct no's
            var allGroupMembersAcctNoList = groupmembers.Select(x => x.AccountNumberString).ToList();

            //if this works cant copy the list 
            var availableGroupMembers = groupmembers.Select(x => x.AccountNumberString).ToList();

            //var randopersonint = rnd.Next(groupmemberacctnolist.Count);
            //var randoperson = groupmemberacctnolist[randopersonint];

            foreach (var g in allGroupMembersAcctNoList)
            {
                //GOTO a BL method to generate a random person. 
                var d = new DrawNamesEditModel();
               


                //var anyconditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(model.Group.GroupId, g);
                //var conditionsList = new List<string>();

                //if (anyconditions.Any())
                //{
                //    foreach (var c in anyconditions)
                //    {
                //        var u = c.UserSelectedForConditionAcctNo;
                //        conditionsList.Add(u);
                //    }
                //}

                // tempAllGroupMembersList.RemoveRange(conditionsList);

                //**Check this out
                //var resultlist = tempAllGroupMembersList.Except(conditionsList).ToList();

                var userone = g;

                var usersnotmelist = availableGroupMembers.Where(x => x != g).ToList();

                //add -1? that could help nvm no it wont
                var randopersonint = rnd.Next(usersnotmelist.Count);

                var usertwo = usersnotmelist[randopersonint];

                // cant remove from a list so need to make it an empty string?
                //allGroupMembersAcctNoList[randopersonint] = string.Empty;

                availableGroupMembers.RemoveAt(randopersonint);

                d.UserOne = userone;
                d.UserTwo = usertwo;
                drawnnamelist.Add(d);

            }

            foreach (var x in drawnnamelist)
            {
                if (x.UserOne == x.UserTwo)
                {
                    redraw = true;   
                }
            }

            if (redraw)
            {
                GetRandomUsersForDrawingNames(groupid);
            }
            

            return drawnnamelist;
        }

    }
}
