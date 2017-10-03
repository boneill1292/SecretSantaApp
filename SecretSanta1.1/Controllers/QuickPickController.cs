using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretSantaApp.BL;
using SecretSantaApp.Views.QuickPick;

namespace SecretSantaApp.Controllers
{
    public class QuickPickController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GroupsController> _log;
        private readonly ISecretSantaBl _secretSantaBl;


        public QuickPickController(ISecretSantaBl secretSantaBl, IHttpContextAccessor httpContextAccessor,
            ILogger<GroupsController> log)
        {
            _secretSantaBl = secretSantaBl;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }


        public ActionResult Index()
        {
            var model = new QuickPickModel();
            return View("QuickPick", model);
        }

        //public ActionResult StartGamePartial()
        //{
        //    return PartialView("_EnterAmountOfPeople")
        //}
    }
}