using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SecretSantaApp.Auth;
using SecretSantaApp.BL;
using SecretSantaApp.Exceptions;
using SecretSantaApp.Models;
using SecretSantaApp.ViewModels;

namespace SecretSantaApp.Controllers
{
    /// <summary>
    ///     For the Login, you will need to return a ChallengeResult and specify "Auth0" as the authentication scheme which
    ///     will be challenged. This will invoke the OIDC middleware you registered in the Configure method.
    ///     After the OIDC middleware has signed the user in, the user will automatically be signed into the cookie middleware
    ///     as well to authenticate them on subsequent requests.So, for the Logout action you will need to sign the user out of
    ///     both the OIDC and the cookie middleware:
    /// </summary>
    public class AccountController : Controller
    {
        private readonly Auth0Settings _auth0Settings;
        private readonly ISecretSantaBl _secretSantaBl;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(IOptions<Auth0Settings> auth0Settings, ISecretSantaBl secretSantaBl,
        UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _auth0Settings = auth0Settings.Value;
            _secretSantaBl = secretSantaBl;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //[HttpGet]
        //public IActionResult Login(string returnUrl = "/")
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //        try
        //        {
        //            var client = new AuthenticationApiClient(new Uri($"https://{_auth0Settings.Domain}/"));

        //            var result = await client.GetTokenAsync(new ResourceOwnerTokenRequest
        //            {
        //                ClientId = _auth0Settings.ClientId,
        //                ClientSecret = _auth0Settings.ClientSecret,
        //                Scope = "openid profile",
        //                Realm = "Username-Password-Authentication", // Specify the correct name of your DB connection
        //                Username = vm.EmailAddress,
        //                Password = vm.Password
        //            });

        //            // Get user info from token
        //            var user = await client.GetUserInfoAsync(result.AccessToken);

        //            var id = user.UserId;
        //            var username = user.PreferredUsername;
        //            var email = user.Email;


        //            // Create claims principal
        //            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        //            {
        //                new Claim(ClaimTypes.NameIdentifier, user.UserId),
        //                new Claim(ClaimTypes.Name, user.FullName)
        //            }, CookieAuthenticationDefaults.AuthenticationScheme));

        //            // Sign user into cookie middleware
        //            await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        //                claimsPrincipal);

        //            return RedirectToLocal(returnUrl);
        //        }
        //        catch (Exception e)
        //        {
        //            ModelState.AddModelError("", e.Message);
        //        }

        //    return View("Login", vm);
        //}

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginViewModel.ReturnUrl))
                        return RedirectToAction("Index", "Home");

                    return Redirect(loginViewModel.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Username/password not found");
            return View("Login",loginViewModel);
        }


        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser() { UserName = loginViewModel.UserName };
                var result = await _userManager.CreateAsync(user, loginViewModel.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View("Register",loginViewModel);
        }



        [HttpGet]
        public IActionResult CheckUser()
        {
            string msg;
            try
            {
                var usermodel = _secretSantaBl.CustomUserModelByLoggedInUser(User);
                //Sends the user to see if it is already in our database, or if should be added
                var model = _secretSantaBl.CheckUserByCustomUserAccountNumber(usermodel);
                //return RedirectToAction(nameof(GroupsController.Index), "Groups");
                return RedirectToAction(nameof(Profile), "Account");
            }
            catch (AppException ax)
            {
                msg = ax.Message;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                msg = "An Error Has Occured";
            }
            return PartialView("_ErrorMessage", new StringModel(msg));


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
                var model = _secretSantaBl.CustomUserDetailsEditModelByAcctNo(acctno);
                return PartialView("_ViewEditUserDetails", model);
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //_log.LogWarning(ex.Message);
            }
            return PartialView("_UserDetails", model);
        }

        [HttpPost]
        public ActionResult SaveUserDetailsInGroup(CustomUserDetailsEditModel model)
        {
            try
            {
                var m = _secretSantaBl.SaveUserDetails(model);
                m.IsMe = true;
                m.Saved = true;
               //return RedirectToAction("ViewOtherUserDetailsPartial", new { acctno = m.UserAcctNo });
                 return PartialView("_ViewEditUserDetails", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                //_log.LogWarning(ex.Message);
            }
            return PartialView("_ViewEditUserDetails", model);
        }


        [HttpGet]
        public IActionResult LoginExternal(string connection, string returnUrl = "/")
        {
            //Sends the user to our RedirectToLocal Action
            var url = Url.Action("RedirectToLocal", "Account");

            var properties = new AuthenticationProperties
            {
                RedirectUri = url
            };

            if (!string.IsNullOrEmpty(connection))
                properties.Items.Add("connection", connection);


            return new ChallengeResult("Auth0", properties);
        }


        public async Task LoginAuth(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index", "Home");
        //}

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }


        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the 
                // **Allowed Logout URLs** settings for the client.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }


        [HttpGet]
        public IActionResult RedirectToLocal(string returnUrl)
        {
            //var url = Url.Action("CheckUser", "Account");
            return RedirectToAction(nameof(CheckUser), "Account");
        }


        //[Authorize]
        //public async Task Logout()
        //{
        //    await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
        //    {
        //        // Indicate here where Auth0 should redirect the user after a logout.
        //        // Note that the resulting absolute Uri must be whitelisted in the 
        //        // **Allowed Logout URLs** settings for the client.
        //        RedirectUri = Url.Action("Index", "Home")
        //    });
        //    await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //}
    }
}