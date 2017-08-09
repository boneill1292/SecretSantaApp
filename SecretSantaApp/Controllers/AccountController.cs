using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecretSantaApp.BL;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.Controllers
{
    /// <summary>
    /// For the Login, you will need to return a ChallengeResult and specify "Auth0" as the authentication scheme which will be challenged. This will invoke the OIDC middleware you registered in the Configure method.
    ///After the OIDC middleware has signed the user in, the user will automatically be signed into the cookie middleware as well to authenticate them on subsequent requests.So, for the Logout action you will need to sign the user out of both the OIDC and the cookie middleware:
    /// </summary>
    public class AccountController : Controller
    {
        private readonly Auth0Settings _auth0Settings;
        private readonly ISecretSantaBl _secretSantaBl;

        public AccountController(IOptions<Auth0Settings> auth0Settings, ISecretSantaBl secretSantaBl)
        {
            _auth0Settings = auth0Settings.Value;
            _secretSantaBl = secretSantaBl;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AuthenticationApiClient client = new AuthenticationApiClient(new Uri($"https://{_auth0Settings.Domain}/"));

                    var result = await client.GetTokenAsync(new ResourceOwnerTokenRequest
                    {
                        ClientId = _auth0Settings.ClientId,
                        ClientSecret = _auth0Settings.ClientSecret,
                        Scope = "openid profile",
                        Realm = "Username-Password-Authentication", // Specify the correct name of your DB connection
                        Username = vm.EmailAddress,
                        Password = vm.Password
                    });

                    // Get user info from token
                    var user = await client.GetUserInfoAsync(result.AccessToken);

                    var id = user.UserId;
                    var username = user.PreferredUsername;
                    var email = user.Email;


                    // Create claims principal
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
            new Claim(ClaimTypes.NameIdentifier, user.UserId),
            new Claim(ClaimTypes.Name, user.FullName)

          }, CookieAuthenticationDefaults.AuthenticationScheme));

                    // Sign user into cookie middleware
                    await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                      claimsPrincipal);

                    return RedirectToLocal(returnUrl);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View("Login", vm);
        }



   



        [HttpGet]
        public IActionResult CheckUser()
        {
            var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
            //Sends the user to see if it is already in our database, or if should be added
            var model = _secretSantaBl.CheckUserByCustomUserAccountNumber(usermodel);
            //return RedirectToAction(nameof(GroupsController.Index), "Groups");
            return RedirectToAction(nameof(AccountController.Profile), "Account");
        }




        [Authorize(Roles = "admin")]
        public IActionResult Admin()
        {
            return View();
        }




        [Authorize]
        [HttpGet]
        public ActionResult Profile()
        {
            string msg;
            try
            {
                var model = _secretSantaBl.UserProfileViewModelByAcctNo(User);
                return View("Profile", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }


        [HttpGet]
        public ActionResult UserDetailsPartial(int userid)
        {
            string msg;
            try
            {
                var model = _secretSantaBl.UserDetailsEditModelByUserId(userid);
                return PartialView("_UserDetails", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }

        [HttpGet]
        public ActionResult ViewOtherUserDetailsPartial(string acctno)
        {
            string msg;
            try
            {

                var model = _secretSantaBl.UserDetailsDisplayModelByAcctNo(acctno);
                return PartialView("_DisplayUserDetails", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                msg = "Error has occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));
        }

        [HttpPost]
        public ActionResult SaveUserDetails(CustomUserDetailsEditModel model)
        {
            try
            {
                var m = _secretSantaBl.SaveUserDetails(model);
                m.Saved = true;
                return PartialView("_UserDetails", m);
                //return PartialView("_JoinGroupEntry", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //_log.LogWarning(ex.Message);
            }
            return PartialView("_UserDetails", model);
        }




        [HttpGet]
        public IActionResult LoginExternal(string connection, string returnUrl = "/")
        {
            //Sends the user to our RedirectToLocal Action
            var url = Url.Action("RedirectToLocal", "Account");
            var properties = new AuthenticationProperties()
            {
                RedirectUri = url
            };

            if (!string.IsNullOrEmpty(connection))
                properties.Items.Add("connection", connection);


            return new ChallengeResult("Auth0", properties);
        }


        [HttpGet]
        public IActionResult RedirectToLocal(string returnUrl)
        {
            //var url = Url.Action("CheckUser", "Account");
            return RedirectToAction(nameof(AccountController.CheckUser), "Account");

        }
        

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        
        
    }
}






