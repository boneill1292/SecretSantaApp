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
using SecretSantaApp.Enumerations;
using SecretSantaApp.Exceptions;
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
        private readonly ICustomUserDetailsDal _customUserDetailsDal;
        private static Random rnd = new Random();
        public SecretSantaBl(IGroupDal groupDal,
                             ICustomUserDal customUserDal,
                             IHttpContextAccessor httpContextAccessor,
                              IGroupMembershipDal groupMembershipDal,
                              IGroupRulesDal groupRulesDal,
                              IGroupMessagesDal groupMessagesDal,
                              IMemberConditionsDal memberConditionsDal,
                               ICustomUserDetailsDal customUserDetailsDal)
        {
            _groupDal = groupDal;
            _customUserDal = customUserDal;
            _httpContextAccessor = httpContextAccessor;
            _groupMembershipDal = groupMembershipDal;
            _groupRulesDal = groupRulesDal;
            _groupMessagesDal = groupMessagesDal;
            _memberConditionsDal = memberConditionsDal;
            _customUserDetailsDal = customUserDetailsDal;
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
                throw new AppException("Password is required");
            }

            var password = group.GroupPassWord;

            if (model.UserInputGroupPassword != password)
            {
                throw new AppException("Incorrect password");
            }
            else
            {
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
            m.InsertedDtm = DateTime.Now;

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


            //Need to look to see if there are any other conditions, and remove them from the result list.
            //** 8/7 - this should be moved to the dropdown - since thats where we generate those people



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
            if (model.UserSelectedForConditionMembershipNo == 0)
            {
                throw new AppException("Please Select A Person");
            }

            var user = GetLoggedInUser();

            var membermodel =
              _groupMembershipDal.GroupMembershipModelByGroupMembershipId(model.UserSelectedForConditionMembershipNo);

            if (membermodel == null)
            {
                throw new AppException("Please Select A Valid Person");
            }

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
            //var condition = _memberConditionsDal.MemberConditionByConditionId(model.ConditionId);
            var condition = new MemberConditions();
            condition.Update(model);

            var deleted = _memberConditionsDal.Delete(condition);

            var result = new MemberConditionsEditModel();
            result.Update(deleted);
            return result;
        }


        public SelectList OtherUsersDropDown(string acctnostr, int groupid)
        {

            var existingconditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(groupid, acctnostr);
            var usersWithConditions = new List<string>();

            if (existingconditions.Any())
            {
                usersWithConditions.AddRange(existingconditions.Select(u => u.UserSelectedForConditionAcctNo));
            }


            var allgroupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);
            var othergroupmembers = allgroupmembers.Where(x => x.AccountNumberString != acctnostr).ToList();

            var resultGroupMemberList = othergroupmembers.Where(x => !usersWithConditions.Contains(x.AccountNumberString)).ToList();


            var rsltlist = new List<GroupConditionsOtherUsersModel>();

            foreach (var o in resultGroupMemberList)
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

            var drawnnamelist = new List<DrawNamesEditModel>();

            try
            {
                drawnnamelist = AssignRandomUsersForGroup(model.Group.GroupId);
            }
            catch (Exception ex)
            {
                throw new AppException("Error drawing names: try again");
            }


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
                throw new AppException("Error: Drawing names is impossible in the current context:");
                GetRandomUsersForDrawingNames(groupid);
            }


            return drawnnamelist;
        }



        public List<DrawNamesEditModel> AssignRandomUsersForGroup(int groupid)
        {
            //var group = _groupDal.GetGroupById(groupid);
            var groupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);


            var result = new List<DrawNamesEditModel>();

            if (groupmembers == null)
            {
                throw new AppException($"Error loading group by ID: {groupid}");
            }

            var membersingroup = groupmembers.Select(x => x.AccountNumberString).ToList();
            var totalavailablemembers = membersingroup;


            foreach (var g in membersingroup)
            {
                var tempavailablemembers = totalavailablemembers;
                var tempmemberswithconditions = new List<string>();
                var dnem = new DrawNamesEditModel();
                dnem.GroupId = groupid;
                dnem.UserOne = g;

                //check for conditions
                var conditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(groupid, g);

                //If the user has any predefined conditions
                if (conditions.Any())
                {
                    //foreach of those conditions add them to a list of users that person x cannot have
                    foreach (var c in conditions)
                    {
                        var conditionalperson = c.UserSelectedForConditionAcctNo;
                        tempmemberswithconditions.Add(conditionalperson);
                    }


                    //remove the conditional users from the list of available people
                    tempavailablemembers = tempavailablemembers.Except(tempmemberswithconditions).ToList();

                    //remove the person we are selecting for from the list
                    tempavailablemembers = tempavailablemembers.Where(x => !x.Contains(g)).ToList();

                    //now get a random person from the list of people left.
                    foreach (var t in tempavailablemembers)
                    {
                        //Throw the random logic in here. The list of people not in condition, and not the user in question.
                    }
                }

                result.Add(dnem);

            }




            return result;
        }


        //Account Stuff
        public UserProfileViewModel UserProfileViewModelByAcctNo(ClaimsPrincipal user)
        {
            //var user = _httpContextAccessor.HttpContext.User;
            var result = new UserProfileViewModel();
            result.Name = user.Identity.Name;
            result.EmailAddress = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            result.ProfileImage = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            result.UserAcctNo = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var u = _customUserDal.CustomUserByAccountNumber(result.UserAcctNo);
            result.UserId = u.UserId;

            return result;

        }


        public CustomUserDetailsEditModel UserDetailsEditModelByUserId(int userid)
        {
            var result = new CustomUserDetailsEditModel();
            //var details = _customUserDetailsDal.UserDetailsByCustomUserAcctNo(acctno);

            var details = _customUserDetailsDal.UserDetailsByUserId(userid);
            if (details != null)
            {
                result.Update(details);
                return result;
            }
            else
            {
                return new CustomUserDetailsEditModel();
            }
        }


        public CustomUserDetailsEditModel UserDetailsEditModelByAcctNo(string acctno)
        {
            var result = new CustomUserDetailsEditModel();
            //var details = _customUserDetailsDal.UserDetailsByCustomUserAcctNo(acctno);

            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            var details = _customUserDetailsDal.UserDetailsByUserId(user.UserId);
            if (details != null)
            {
                result.Update(details);
                return result;
            }
            else
            {
                return new CustomUserDetailsEditModel();
            }
        }

        public CustomUserDetailsDisplayModel UserDetailsDisplayModelByAcctNo(string acctno)
        {
            var result = new CustomUserDetailsDisplayModel();
            //var details = _customUserDetailsDal.UserDetailsByCustomUserAcctNo(acctno);

            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            result.UserFullName = user.FullName;

            var details = _customUserDetailsDal.UserDetailsByUserId(user.UserId);
            if (details != null)
            {
                result.Update(details);
            }

            return result;

        }


        public CustomUserDetailsEditModel SaveUserDetails(CustomUserDetailsEditModel model)
        {

            var result = new CustomUserDetailsEditModel();
            var details = new CustomUserDetails();
            details.Update(model);
            var saved = _customUserDetailsDal.Save(details);
            //result.Update(saved);
            result.Update(saved);
            return result;
        }


        public List<SelectListItem> CommonSizesDropdown()
        {
            var deptList = new List<SelectListItem>();
            deptList.Add(new SelectListItem
            {
                Text = "Please Select a Size",
                Value = ""
            });
            foreach (var eVal in Enum.GetValues(typeof(CommonSizes)))
            {
                deptList.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(CommonSizes), eVal).ToUpper(),
                    Value = eVal.ToString().ToUpper()
                });
            }
            return deptList;
            //var commonSizes = Enum.GetValues(typeof(CommonSizes)).Cast<CommonSizes>().ToList();

            //var rsltlist = new List<GroupConditionsOtherUsersModel>();


            //var result = new SelectList(commonSizes, nameof(CommonSizes..MembershipId),
            //    nameof(GroupConditionsOtherUsersModel.FullName));


            //return result;
        }
    }
}
