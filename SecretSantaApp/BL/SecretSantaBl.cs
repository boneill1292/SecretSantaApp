using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecretSantaApp.DAL;
using SecretSantaApp.Enumerations;
using SecretSantaApp.Exceptions;
using SecretSantaApp.Models;
using SecretSantaApp.Services;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.BL
{
    public class SecretSantaBl : ISecretSantaBl
    {
        private static Random rnd = new Random();
        private readonly ICustomUserDal _customUserDal;
        private readonly ICustomUserDetailsDal _customUserDetailsDal;

        private readonly IGroupDal _groupDal;
        private readonly IGroupMembershipDal _groupMembershipDal;
        private readonly IGroupMessagesDal _groupMessagesDal;
        private readonly IGroupPairingsDal _groupPairingsDal;
        private readonly IGroupRulesDal _groupRulesDal;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemberConditionsDal _memberConditionsDal;
        private readonly IViewRenderService _viewRenderService;

        public SecretSantaBl(IGroupDal groupDal,
            ICustomUserDal customUserDal,
            IHttpContextAccessor httpContextAccessor,
            IGroupMembershipDal groupMembershipDal,
            IGroupRulesDal groupRulesDal,
            IGroupMessagesDal groupMessagesDal,
            IMemberConditionsDal memberConditionsDal,
            IGroupPairingsDal groupPairingDal,
            IViewRenderService viewRenderService,
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
            _viewRenderService = viewRenderService;
            _groupPairingsDal = groupPairingDal;
        }

        public CustomUserEditModel CustomUserModelByLoggedInUser(ClaimsPrincipal user)
        {
            var acctid = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var testfromvscode = "second vs code test";



            //resharper ignore nullcheck
            if (acctid == null)
                throw new AppException("Error getting account number");

            var existinguser = _customUserDal.CustomUserByAccountNumber(acctid);

            if (existinguser.AccountNumberString != null)
            {
                var existingusereditmodel = new CustomUserEditModel();
                existingusereditmodel.Update(existinguser);
                existingusereditmodel.NewUser = false;
                return existingusereditmodel;
            }


            var result = new CustomUserEditModel();
            var name = user.Identity.Name;
            var email = user.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            if (email == null)
                throw new AppException("Error getting email from auth0");

            var pic = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            result.AccountNumberString = acctid;
            result.FullName = name;
            result.Email = email;
            result.ProfileImage = pic;
            result.NewUser = true;
            result.UserId = 0;
            return result;
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

        public GroupEditModel GroupEditModelByGroupId(int id)
        {
            var group = _groupDal.GetGroupById(id);

            if (group == null)
                throw new AppException($"Error loading Group by ID: {id}");

            var result = new GroupEditModel();
            result.Update(group);
            result.GroupId = group.GroupId;

            return result;
        }

        public GroupEditModel SaveNewGroup(GroupEditModel model)
        {
            //Check to make sure there are values
            if (model.GroupName == null)
                throw new AppException("Name is Required");


            // var group = _groupDal.GetGroupById(model.GroupId);

            //if (group == null)
            //    throw new AppException($"Error loading Group ID: {model.GroupId}");


            //code to get the currently logged in user
            //var liu = _httpContextAccessor.HttpContext.User;
            //var u = CustomUserModelByLoggedInUser(liu);
            var u = GetLoggedInUser();

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

        public GroupEditModel SaveGroup(GroupEditModel model)
        {
            if (model.GroupName == null)
                throw new Exception("Name is Required");


            var group = new Group();
            group.Update(model);
            group.GroupId = model.GroupId;

            var saved = _groupDal.SaveGroup(group);
            model.Update(saved);

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
            return model;
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

            var groupsibelongto =
                _groupMembershipDal.GroupsBelongingToUserAccountNumberString(user.AccountNumberString);

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
            result.GroupAdminBool = group.InsertedBy == loggedinuser.AccountNumberString;

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


            if (groupmembership.Any(x => x.AccountNumberString == loggedinuser.AccountNumberString))
                result.IsAuthorized = true;
            else
                result.IsAuthorized = false;


            //check to see if the pairings have been assigned: (AKA - You have your secret santa)
            var pairings = _groupPairingsDal.GroupPairingsByGroupId(groupid);

            if (pairings.Count >= 1)
                result.PairingsAssigned = true;
            else
                result.PairingsAssigned = false;


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
                activegroupsidlist.Add(ag.GroupId);

            foreach (var ag in groupsmemberof)
                groupsmemberofidlist.Add(ag.GroupId);


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


        public InviteUsersEditModel InviteUsersEditModelByGroupId(int groupid)
        {
            var result = new InviteUsersEditModel();


            //Assuming that at least 4 people will want to be invited.
            result.InviteUsersCollection = InviteUsersCollectionModelByAmountToGet(4, groupid);


            return result;
        }

        public InviteUsersCollectionModel InviteUsersCollectionModelByAmountToGet(int amount, int groupid)
        {
            var result = new InviteUsersCollectionModel();
            result.UsersToInvite = new List<InviteUsersViewModel>();

            var group = _groupDal.GetGroupById(groupid);

            if (group == null)
                throw new AppException($"Error Loading Group By ID: {groupid}");

            var url = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();


            // UrlHelperExtensions.Action("GroupHome", "Groups", new { id = groupid });
            // Microsoft.AspNetCore.Http.Extensions.UriHelper.GetFullUrl(Request)

            // var url = _httpContext.Request.GetEncodedUrl();

            if (amount <= 0)
                return result;

            for (var a = 0; a < amount; a++)
            {
                var usertoinvite = new InviteUsersViewModel();
                usertoinvite.GroupId = groupid;
                usertoinvite.GroupName = group.GroupName;
                usertoinvite.GroupUrl = url;
                usertoinvite.GroupPassword = group.GroupPassWord;
                result.UsersToInvite.Add(usertoinvite);
            }
            return result;
        }


        public InviteUsersViewModel AdditionalInviteUsersViewModel(int tempid, int groupid)
        {
            var result = new InviteUsersViewModel();
            result.TempId = tempid;
            result.GroupId = groupid;
            return result;
        }


        public async Task<InviteUsersEditModel> SendInviteToUsersAsync(InviteUsersEditModel model)
        {
            var result = new InviteUsersEditModel();

            if (model.InviteUsersCollection.UsersToInvite.All(x => x.Name == null))
                throw new AppException("You must at least invite 1 person");
            // var email = "knowyaflow@gmail.com";

            // var please = SendAsync();


            // SendEmailAsync(email, "invite", "do you wanna play");
            //SendSimpleMessge();

            //var message = new MimeMessage();
            //message.From.Add(new MailboxAddress("Santa", "santa@elfbuddies.com  "));
            //message.To.Add(new MailboxAddress("Ben", "boneill1292@gmail.com"));
            //message.Subject = "Invite to Elf Buddies!";
            //message.Body = new TextPart("plain")
            //{
            //    Text = "Hello Ben. Want to join elf buddies?"
            //};

            //using (var client = new SmtpClient())
            //{
            //    client.Connect("smtp.example.com", 587, false);
            //    client.AuthenticationMechanisms.Remove("XOAUTH2");
            //    // Note: since we don't have an OAuth2 token, disable 	// the XOAUTH2 authentication mechanism.     client.Authenticate("anuraj.p@example.com", "password");
            //    client.Send(message);
            //    client.Disconnect(true);
            //}

            foreach (var i in model.InviteUsersCollection.UsersToInvite)
                //only send it if the name has been filled out
                if (i.Name != null)
                {
                    //This is how it works. keep experimenting 
                    i.GroupUrl = model.GroupUrl;
                    await SendInviteEmailAsync(i).ConfigureAwait(false);
                    ////Email.DefaultRenderer = new RazorRenderer();


                    ////var recipient = i.Email;


                    ////var emailtwo = Email
                    ////    .From("Santa@ElfBuddies.Com")
                    ////    .To(recipient)
                    ////    .Subject("Invite To Play")
                    ////    .UsingTemplateFromFile("Views/Shared/_InviteUsersEmailTemplate.cshtml", i, true);

                    ////emailtwo.Send();
                }

            return result;
        }


        public void GetContextFromController(ContentResult content)
        {
        }


        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    var emailMessage = new MimeMessage();

        //    emailMessage.From.Add(new MailboxAddress("Joe Bloggs", "jbloggs@example.com"));
        //    emailMessage.To.Add(new MailboxAddress("", email));
        //    emailMessage.Subject = subject;
        //    emailMessage.Body = new TextPart("plain") { Text = message };

        //    using (var client = new SmtpClient())
        //    {
        //        client.LocalDomain = "http://elfbuddies.com";
        //        await client.ConnectAsync("smtp.relay.uri", 25, SecureSocketOptions.None).ConfigureAwait(false);
        //        await client.SendAsync(emailMessage).ConfigureAwait(false);
        //        await client.DisconnectAsync(true).ConfigureAwait(false);
        //    }
        //}


        //    public void SendSimpleMessge()
        //    {

        //        RestClient client = new RestClient();
        //        client.BaseUrl = new Uri("https://api.mailgun.net/v3/elfbuddies.com");
        //        client.Authenticator = new HttpBasicAuthenticator("api",
        //                "key-30e16c6964d4f339fab512a5aa3b988d");
        //        RestRequest request = new RestRequest();
        //        request.AddParameter("domain", "elfbuddies.com", ParameterType.UrlSegment);
        //        request.Resource = "elfbuddies.com/messages";
        //        request.AddParameter("from", "Excited User <santa@elfbuddies.com>");
        //        request.AddParameter("to", "boneill1292@gmail.com");
        //        request.AddParameter("to", "santa@elfbuddies.com");
        //        request.AddParameter("subject", "Hello");
        //        request.AddParameter("text", "Testing some Mailgun awesomness!");
        //        request.Method = Method.POST;
        //        client.ExecuteAsync<RestClient>(request, (response) =>
        //{

        //});


        //    }

        //public async IRestResponse SendSimpleMessage()
        //{
        //    RestClient client = new RestClient();
        //    client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        //    client.Authenticator =
        //        new HttpBasicAuthenticator("api",
        //            "YOUR_API_KEY");
        //    RestRequest request = new RestRequest();
        //    request.AddParameter("domain", "YOUR_DOMAIN_NAME", ParameterType.UrlSegment);
        //    request.Resource = "{domain}/messages";
        //    request.AddParameter("from", "Excited User <mailgun@YOUR_DOMAIN_NAME>");
        //    request.AddParameter("to", "bar@example.com");
        //    request.AddParameter("to", "YOU@YOUR_DOMAIN_NAME");
        //    request.AddParameter("subject", "Hello");
        //    request.AddParameter("text", "Testing some Mailgun awesomness!");
        //    request.Method = Method.POST;
        //    return client.ExecuteAsync(request, "hi");
        //}


        //public static IRestResponse SendSimpleMessage()
        //{
        //    RestClient client = new RestClient();
        //    client.BaseUrl = new Uri("https://api.mailgun.net/v3");
        //    client.Authenticator =
        //        new HttpBasicAuthenticator("api",
        //            "YOUR_API_KEY");
        //    RestRequest request = new RestRequest();
        //    request.AddParameter("domain", "YOUR_DOMAIN_NAME", ParameterType.UrlSegment);
        //    request.Resource = "{domain}/messages";
        //    request.AddParameter("from", "Excited User <mailgun@YOUR_DOMAIN_NAME>");
        //    request.AddParameter("to", "bar@example.com");
        //    request.AddParameter("to", "YOU@YOUR_DOMAIN_NAME");
        //    request.AddParameter("subject", "Hello");
        //    request.AddParameter("text", "Testing some Mailgun awesomness!");
        //    request.Method = Method.POST;
        //    return client.(request);
        //}

        public JoinGroupEditModel JoinGroupEditModelByGroupId(int groupid)
        {
            var group = _groupDal.GetGroupById(groupid);

            if (group == null)
                throw new Exception("Error loading group");


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
                throw new Exception("Error loading group");

            if (model.UserInputGroupPassword == null)
                throw new AppException("Password is required");

            var password = group.GroupPassWord;

            if (model.UserInputGroupPassword != password)
                throw new AppException("Incorrect password");
            model.GroupId = group.GroupId;
            model.GroupName = group.GroupName;

            JoinGroupAsCustomUser(model.CustomUser, model.GroupId);
            model.Verified = true;
            model.ErrorMsg = null;
            return model;
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
                throw new Exception("Required");

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
                throw new Exception("Required");

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
            othergroupmembers = othergroupmembers.Where(x => x.AccountNumberString != members.AccountNumberString)
                .ToList();


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
                throw new AppException("Please Select A Person");

            var user = GetLoggedInUser();

            var membermodel =
                _groupMembershipDal.GroupMembershipModelByGroupMembershipId(model.UserSelectedForConditionMembershipNo);

            if (membermodel == null)
                throw new AppException("Please Select A Valid Person");

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
                usersWithConditions.AddRange(existingconditions.Select(u => u.UserSelectedForConditionAcctNo));


            var allgroupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);
            var othergroupmembers = allgroupmembers.Where(x => x.AccountNumberString != acctnostr).ToList();

            var resultGroupMemberList =
                othergroupmembers.Where(x => !usersWithConditions.Contains(x.AccountNumberString)).ToList();


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


        public async Task<DrawNamesDisplayModel> DrawNamesAsync(DrawNamesDisplayModel model)
        {
            var drawnnamelist = new List<DrawNamesEditModel>();
            var maxretries = 100;
            var retries = 0;

            while (true)
                try
                {
                    drawnnamelist = AssignRandomUsersForGroup(model.Group.GroupId);
                    break;
                }

                catch (Exception ex)
                {
                    Console.WriteLine(" - " + ex.Message);


                    if (retries < maxretries)
                        retries++;
                    else
                        throw new AppException("Please alter your conditions to make the group solvable");
                }


            model.DrawNamesList = new List<DrawNamesEditModel>();
            model.DrawNamesList = drawnnamelist;


            await EmailGroupMembersResultsAsync(model).ConfigureAwait(false);
            return model;
        }


        private async Task EmailGroupMembersResultsAsync(DrawNamesDisplayModel model)
        {
            foreach (var n in model.DrawNamesList)
            {
                var emailmodel = new EmailDrawnNamesUpdateModel();
                emailmodel.GroupName = model.Group.GroupName;

                var personemailing = _customUserDal.CustomUserByAccountNumber(n.PersonOne);
                var persontheyreceived = _customUserDal.CustomUserByAccountNumber(n.PersonTwo);
                emailmodel.PersonOneName = personemailing.FullName;
                emailmodel.PersonOneEmail = personemailing.Email;

                emailmodel.PersonTwoName = persontheyreceived.FullName;
                emailmodel.PersonTwoEmail = persontheyreceived.Email;

                await SendPariedMemberResultsAsync(emailmodel).ConfigureAwait(false);

            }
        }

        private async Task<bool> SendPariedMemberResultsAsync(EmailDrawnNamesUpdateModel i)
        {
            var emailbody = $"Yo {i.PersonOneName}!  \n" +
                            $"We just drew names in Group: {i.GroupName} and you got: {i.PersonTwoName}. \n "

                ;

            var ms = new MailService();

            var from = "santa@elfbuddies.com";
            var to = i.PersonOneEmail;
            var subject = "We Drew Names!";


            //Get the razor view here
            //https://stackoverflow.com/questions/40912375/return-view-as-string-in-net-core
            var mailto = await ms.SendAsync(@from, to, subject, emailbody).ConfigureAwait(false);

            return mailto;
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


        public GroupPairingDisplayModel GroupPairingDisplayModelByLoggedInUserByGroupId(int groupid)
        {
            var result = new GroupPairingDisplayModel();

            var pairs = _groupPairingsDal.GroupPairingsByGroupId(groupid);

            var me = GetLoggedInUser();

            var mypair = pairs.FirstOrDefault(x => x.PersonOne == me.AccountNumberString);

            if (mypair != null)
            {
                var pairacctno = mypair.PersonTwo;
                var person = _customUserDal.CustomUserByAccountNumber(pairacctno);

                result.PairedMemberString = person.FullName;
            }
            else
            {
                result.PairedMemberString = "The group has not drawn yet";
            }


            return result;
        }


        //Account Stuff
        public UserProfileViewModel UserProfileViewModelByAcctNo(ClaimsPrincipal user)
        {
            //var user = _httpContextAccessor.HttpContext.User;
            var result = new UserProfileViewModel();
            result.Name = user.Identity.Name;

            //          var testg = user.Claims.FirstOrDefault(x=> x.Type == ClaimsIdentit)
            result.EmailAddress = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            result.ProfileImage = user.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            result.UserAcctNo = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var u = _customUserDal.CustomUserByAccountNumber(result.UserAcctNo);
            result.UserId = u.UserId;

            result.Claims = user.Claims.ToList();

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
            return new CustomUserDetailsEditModel();
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
            return new CustomUserDetailsEditModel();
        }

        public CustomUserDetailsDisplayModel UserDetailsDisplayModelByAcctNo(string acctno)
        {
            var result = new CustomUserDetailsDisplayModel();
            //var details = _customUserDetailsDal.UserDetailsByCustomUserAcctNo(acctno);

            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            result.UserFullName = user.FullName;

            var details = _customUserDetailsDal.UserDetailsByUserId(user.UserId);
            if (details != null)
                result.Update(details);

            return result;
        }

        public CustomUserDetailsEditModel CustomUserDetailsEditModelByAcctNo(string acctno)
        {
            var result = new CustomUserDetailsEditModel();

            var loggedinuser = GetLoggedInUser();

            if (loggedinuser.AccountNumberString == acctno)
            {
                result.IsMe = true;

            }

            var user = _customUserDal.CustomUserByAccountNumber(acctno);

            result.UserFullName = user.FullName;
            result.UserAcctNo = acctno;
            result.UserId = user.UserId;

            var details = _customUserDetailsDal.UserDetailsByUserId(user.UserId);
            if (details != null)
                result.Update(details);


            //var details = _customUserDetailsDal.UserDetailsByCustomUserAcctNo(acctno);
            //result.Update(details);


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
                deptList.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(CommonSizes), eVal).ToUpper(),
                    Value = eVal.ToString().ToUpper()
                });
            return deptList;


            //return result;
        }

        //This method works
        private async Task<bool> SendInviteEmailAsync(InviteUsersViewModel i)
        {
            var emailbody = $"Yo {i.Name}!  \n" +
                            $"You were invited to join group: {i.GroupName}. \n " +
                            $"Click Here To Join The Group: {i.GroupUrl}" + "\n" +
                            $"The Password is: {i.GroupPassword}" + "\n";

            var ms = new MailService();

            var from = "santa@elfbuddies.com";
            var to = i.Email;
            var subject = "Invite to Join";
            var body = "hello world from mailgun";


            //Get the razor view here
            //https://stackoverflow.com/questions/40912375/return-view-as-string-in-net-core
            var mailto = await ms.SendAsync(@from, to, subject, emailbody).ConfigureAwait(false);

            return mailto;
        }





        //public List<DrawNamesEditModel> GetRandomUsersForDrawingNames(int groupid)
        //{
        //    var drawnnamelist = new List<DrawNamesEditModel>();
        //    bool redraw = false;

        //    //Load all the members in a group
        //    var groupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);

        //    //Get a list of all members acct no's
        //    var allGroupMembersAcctNoList = groupmembers.Select(x => x.AccountNumberString).ToList();

        //    //if this works cant copy the list 
        //    var availableGroupMembers = groupmembers.Select(x => x.AccountNumberString).ToList();

        //    //var randopersonint = rnd.Next(groupmemberacctnolist.Count);
        //    //var randoperson = groupmemberacctnolist[randopersonint];

        //    foreach (var g in allGroupMembersAcctNoList)
        //    {
        //        //GOTO a BL method to generate a random person. 
        //        var d = new DrawNamesEditModel();


        //        //var anyconditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(model.Group.GroupId, g);
        //        //var conditionsList = new List<string>();

        //        //if (anyconditions.Any())
        //        //{
        //        //    foreach (var c in anyconditions)
        //        //    {
        //        //        var u = c.UserSelectedForConditionAcctNo;
        //        //        conditionsList.Add(u);
        //        //    }
        //        //}

        //        // tempAllGroupMembersList.RemoveRange(conditionsList);

        //        //**Check this out
        //        //var resultlist = tempAllGroupMembersList.Except(conditionsList).ToList();

        //        var userone = g;

        //        var usersnotmelist = availableGroupMembers.Where(x => x != g).ToList();

        //        //add -1? that could help nvm no it wont
        //        var randopersonint = rnd.Next(usersnotmelist.Count);

        //        var usertwo = usersnotmelist[randopersonint];

        //        // cant remove from a list so need to make it an empty string?
        //        //allGroupMembersAcctNoList[randopersonint] = string.Empty;

        //        availableGroupMembers.RemoveAt(randopersonint);

        //        d.PersonOne = userone;
        //        d.PersonTwo = usertwo;
        //        drawnnamelist.Add(d);

        //    }

        //    foreach (var x in drawnnamelist)
        //    {
        //        if (x.PersonOne == x.PersonTwo)
        //        {
        //            redraw = true;
        //        }
        //    }

        //    if (redraw)
        //    {
        //        throw new AppException("Error: Drawing names is impossible in the current context:");
        //        GetRandomUsersForDrawingNames(groupid);
        //    }


        //    return drawnnamelist;
        //}


        private List<DrawNamesEditModel> AssignRandomUsersForGroup(int groupid)
        {
            //var group = _groupDal.GetGroupById(groupid);
            var groupmembers = _groupMembershipDal.AllGroupMembersByGroupId(groupid);


            var result = new List<DrawNamesEditModel>();

            if (groupmembers == null)
                throw new AppException($"Error loading group by ID: {groupid}");

            //The members have been loaded, clear out existing pairings so we can assign new ones.
            ClearGroupPairingsByGroupId(groupid);


            var membersingroup = groupmembers.Select(x => x.AccountNumberString).ToList();
            var totalavailablemembers = membersingroup;


            foreach (var g in membersingroup)
            {
                var tempavailablemembers = totalavailablemembers;
                var tempmemberswithconditions = new List<string>();
                var dnem = new DrawNamesEditModel();
                dnem.GroupId = groupid;
                dnem.PersonOne = g;

                //check for conditions
                var conditions = _memberConditionsDal.MemberConditionsByGroupIdByAcctNo(groupid, g);

                //If the user has any predefined conditions
                if (conditions.Any())
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
                var r = new Random();

                var rando = tempavailablemembers.ElementAt(r.Next(0, tempavailablemembers.Count()));

                dnem.PersonTwo = rando;

                //remove the user who we just assigned to user two from the global list of available people
                totalavailablemembers = totalavailablemembers.Where(x => !x.Contains(rando)).ToList();

                result.Add(dnem);


                //all the logic has been assigned, now save the record.
                _groupPairingsDal.Save(dnem);
            }


            return result;
        }

        private void ClearGroupPairingsByGroupId(int groupid)
        {
            var grouppairings = _groupPairingsDal.GroupPairingsByGroupId(groupid);

            if (grouppairings.Count >= 1)
                foreach (var g in grouppairings)
                    _groupPairingsDal.Delete(g);
        }
    }
}