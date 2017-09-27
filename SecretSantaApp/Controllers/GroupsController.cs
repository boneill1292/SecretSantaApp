using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretSantaApp.BL;
using SecretSantaApp.Exceptions;
using SecretSantaApp.Models;
using SecretSantaApp.Services;
using SecretSantaApp.ViewModels;
using SecretSantaApp.Views.Groups;

namespace SecretSantaApp.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupsController> _log;
        private readonly ISecretSantaBl _secretSantaBl;
        private readonly IViewRenderService _viewRenderService;


        public GroupsController(ISecretSantaBl secretSantaBl, IHttpContextAccessor httpContextAccessor, IViewRenderService viewRenderService,
            ILogger<GroupsController> log)
        {
            _secretSantaBl = secretSantaBl;
            _httpContextAccessor = httpContextAccessor;
            _viewRenderService = viewRenderService;
            _log = log;
        }

        [HttpGet]
        // [Route("benapp/test")]
        public ActionResult Index()
        {
            //  var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
            var model = _secretSantaBl.DefaultGroupAdminModel();
        return View("Index", model);
        }


        [HttpGet]
        [Authorize]
        // [Route("benapp/test")]
        public ActionResult MyGroups()
        {
            var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
            var usergroupsvm = _secretSantaBl.MyGroupsViewModelByUserId(usermodel);


            return View("MyGroups", usergroupsvm);
        }


        [HttpGet]
        //[Route("groups/group/_defaultsearchresult")]
        public ActionResult GroupHome(int id)
        {
            var msg = "";
            try
            {
                var model = _secretSantaBl.GroupHomeEditModelByGroupId(id);
                //model.InviteUsersCollection = _secretSantaBl.InviteUsersCollectionModelByAmountToGet(4);

                return View("GroupHome", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogError(" - ", ex.Message);
            }
            return View("_ErrorMessage", new StringModel(msg));
            // need to make a full page error view but o well
        }

        [HttpGet]
       public ActionResult GetGroupName(int id)
        {
            var msg = "";
            try
            {
                var model = _secretSantaBl.GroupEditModelByGroupId(id);
                return PartialView("_GroupName", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogError(" - ", ex.Message);
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult NewGroupPage()
        {
            string msg;
            try
            {
                var model = _secretSantaBl.DefaultGroupEditModel();
                return View("NewGroup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return View("_ErrorMessage", new StringModel(msg));
        }

        [HttpGet]
        public ActionResult EditGroup(int id)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupEditModelByGroupId(id);
                return PartialView("_EditGroupPopup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }



        [HttpPost]
        //[Route("tickets/{department}/newcategoryonticketview/edit")]
        public ActionResult SaveNewGroup(GroupEditModel model)
        {
            if (!ModelState.IsValid)
                return View("NewGroup", model);
            try
            {
                //save the new group - get the ID
                var m = _secretSantaBl.SaveNewGroup(model);
                var grouphome = _secretSantaBl.GroupHomeEditModelByGroupId(m.GroupId);
                grouphome.NewGroup = true;

                return RedirectToAction("GroupHome", new {id = m.GroupId});
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogWarning(ex.Message);
            }
            return View("NewGroup", model);
        }



        [HttpPost]
        //[Route("tickets/{department}/newcategoryonticketview/edit")]
        public ActionResult SaveGroup(GroupEditModel model)
        {
            if (!ModelState.IsValid)
                return View("NewGroup", model);
            try
            {
                //save the new group - get the ID
                var m = _secretSantaBl.SaveGroup(model);
                m.Saved = true;
                return PartialView("_EditGroupPopup", m);
                //return RedirectToAction("GroupHome", new { id = m.GroupId });
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogWarning(ex.Message);
            }
            return PartialView("_EditGroupPopup", model);
        }
        //End InviteUsersRegion

        [HttpGet]
        public ActionResult AvailableGroupsToJoin()
        {
            string msg;
            try
            {
                var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
                var model = _secretSantaBl.JoinGroupEditModelByAccountNumberString(usermodel.AccountNumberString);

                return View("JoinGroups", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        [Route("secretsanta/joingroup/{id}")]
        public ActionResult PromptUserForPassword(int id)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.JoinGroupEditModelByGroupId(id);
                return PartialView("_JoinGroupEntry", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpPost]
        public ActionResult SubmitJoinGroup(JoinGroupEditModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_JoinGroupEntry", model);
            //We need to pass the correct password - if the user does that.Add them to the group, and load the group  homepage.
            try
            {
                var liu = _httpContextAccessor.HttpContext.User;
                var u = _secretSantaBl.CustomUserModelByLoggedInUser(liu);
                model.CustomUser = u;
                var m = _secretSantaBl.CheckPasswordInput(model);
                return PartialView("_JoinGroupEntry", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogWarning(ex.Message);
            }
            return PartialView("_JoinGroupEntry", model);
        }


        [HttpGet]
        public ActionResult GetManageRulesPopup(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.NewRuleEditModelByGroupId(groupid);
                return PartialView("_NewRulePopup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult EditRulePopup(int ruleid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupRuleEditModelByRuleId(ruleid);
                return PartialView("_NewRulePopup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult DeleteRulePopup(int ruleid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupRuleEditModelByRuleId(ruleid);
                return PartialView("_DeleteRulePopup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }

        [HttpPost]
        public ActionResult DeleteGroupRule(GroupRulesEditModel model)
        {
            try
            {
                //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
                //var m = _secretSantaBl.SaveGroupRules(model);
                var m = _secretSantaBl.DeleteGroupRule(model);
                m.Saved = true;
                return PartialView("_DeleteRulePopup", m);
                //return PartialView("_JoinGroupEntry", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogError("", ex);
            }
            return PartialView("_DeleteRulePopup", model);
        }


        [HttpPost]
        public ActionResult SaveGroupRule(GroupRulesEditModel model)
        {
            try
            {
                //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
                var m = _secretSantaBl.SaveGroupRules(model);
                m.Saved = true;
                return PartialView("_NewRulePopup", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //_log.Error(ex);
            }
            return PartialView("_NewRulePopup", model);
        }


        [HttpGet]
        public ActionResult GetGroupRulesPartial(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupRulesDisplayModelByGroupId(groupid);
                return PartialView("_GroupRules", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult GetChatPartial(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupChatDisplayModelByGroupId(groupid);
                return PartialView("_GroupChat", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult NewMessagePartial(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.NewGroupMessageEditModelByGroupId(groupid);
                return PartialView("_NewMessage", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpPost]
        public ActionResult SaveGroupMessage(GroupMessageEditModel model)
        {
            try
            {
                //var u = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<CustomUserEditModel>("LoggedInUser");
                // var m = _secretSantaBl.SaveGroupRules(model);
                var m = _secretSantaBl.SaveGroupMessage(model);
                m.Saved = true;
                return PartialView("_NewMessage", m);
                //return PartialView("_JoinGroupEntry", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return PartialView("_NewMessage", model);
        }


        [HttpGet]
        public ActionResult NewMemberConditionPopup(int membershipid, string acctno)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.MemberConditionsEditModelByMembershipId(membershipid, acctno);
                model.NewCondition = true;
                return PartialView("_MemberConditions", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpPost]
        public ActionResult SaveNewMemberCondition(MemberConditionsEditModel model)
        {
            try
            {
                model.ConditionId = 0;
                var m = _secretSantaBl.SaveNewMemberCondition(model);
                m.Saved = true;

                return PartialView("_MemberConditions", model);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogWarning(ex.Message);
            }
            return PartialView("_MemberConditions", model);
        }


        [HttpGet]
        public ActionResult DeleteConditionPopup(int conditionid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.MemberConditionsEditModelByConditionId(conditionid);
                return PartialView("_DeleteConditionPopup", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpPost]
        public ActionResult DeleteCondition(MemberConditionsEditModel model)
        {
            try
            {
                var m = _secretSantaBl.DeleteMemberCondition(model);
                m.Saved = true;
                return PartialView("_DeleteConditionPopup", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return PartialView("_DeleteConditionPopup", model);
        }


        [HttpGet]
        public ActionResult GetDrawNamesPartial(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.DrawNamesDisplayModelByGroupId(groupid);
                return PartialView("_DrawNames", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpPost]
        public ActionResult SubmitDrawNames(DrawNamesDisplayModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_DrawNames", model);
            try
            {
                var m = _secretSantaBl.DrawNames(model);
                m.Saved = true;
                return PartialView("_DrawNames", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogWarning(ex.Message);
            }
            return PartialView("_DrawNames", model);
        }


        [HttpGet]
        public ActionResult DisplayGroupPairedMember(int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.GroupPairingDisplayModelByLoggedInUserByGroupId(groupid);
                return PartialView("_GroupPairedMember", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        #region InviteUsersRegion

        [HttpGet]
        public ActionResult InviteUsersPartialView(int groupid)
        {
            string msg;
            try
            {
           
                var model = _secretSantaBl.InviteUsersEditModelByGroupId(groupid);
                return PartialView("_InviteUsers", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult GetInviteUsersRow(int count, int groupid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.AdditionalInviteUsersViewModel(count, groupid);
                
                return PartialView("_InviteUsersRow", model);
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception)
            {
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }

        [HttpPost]
        public ActionResult SendInvitesTousers(InviteUsersEditModel model)
        {
            try
            {
                
                var url = Url.Action("GroupHome", "Groups", new {id = model.GroupId});
                model.GroupUrl = url;
                var m = _secretSantaBl.SendInviteToUsers(model);
                m.Saved = true;
                return PartialView("_InviteUsers", m);
            }
            catch (AppException ax)
            {
                ModelState.AddModelError("", ax.AppMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.LogError("", ex);
            }
            return PartialView("_InviteUsers", model);
        }


        //[Route("invite")]
        //public async Task<IActionResult> RenderInviteView()
        //{
        //    ViewData["Message"] = "Your application description page.";

        //    var ivm = new InviteUsersViewModel();

        //    ivm.Email = "knwoyafklow@gmail.com";
        //    ivm.GroupName = "hi";
        //    ivm.Name = "bo janglez";

        //    var result = await _viewRenderService.RenderToStringAsync("Shared/_InviteUsersEmailTemplate", ivm);

        //    var str = Content(result);

        //    _secretSantaBl.GetContextFromController(str);
        //    return Content(result);
        //}
    }


    #endregion
    // End Invite Users
}
