using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApp.ViewModels;

// Needed for the SetString and GetString extension methods

namespace SecretSantaApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ISecretSantaBl _secretSantaBl;

        [HttpGet]
        // [Route("home/index")]
        public ActionResult Index()
        {
            //HttpContext.Session.SetString("Test", "Ben Rules!");  
            //var model = _secretSantaBl.DefaultTestDataViewModel();
            return View("Index");
        }


        [Authorize]
        public IActionResult About()
        {
            var test = HttpContext.Session.GetObjectFromJson<TestDataViewModel>("Test");
            ViewData["Message"] = "Your application description page.";

            return View("About");
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View("Contact");
        }

        public IActionResult Error()
        {
            var exception = HttpContext.Features
                .Get<IExceptionHandlerFeature>();

            ViewData["statusCode"] = HttpContext.Response.StatusCode;
            ViewData["message"] = exception.Error.Message;
            ViewData["stackTrace"] = exception.Error.StackTrace;


            return View("Error");
        }
    }
}