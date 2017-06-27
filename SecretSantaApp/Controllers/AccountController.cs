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

    public AccountController(IOptions<Auth0Settings> auth0Settings)
    {
      _auth0Settings = auth0Settings.Value;
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
      //Explore this?
      //if (User.Identity.IsAuthenticated)
      //{
      //  var authenticateInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync("Auth0");
      //  string accessToken = authenticateInfo.Properties.Items[".Token.access_token"];
      //  string idToken = authenticateInfo.Properties.Items[".Token.id_token"];
      //}
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

      return View(vm);
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

    private IActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction(nameof(HomeController.Index), "Home");
      }
    }



    [HttpGet]
    public IActionResult LoginExternal(string connection, string returnUrl = "/")
    {
      var properties = new AuthenticationProperties() { RedirectUri = returnUrl };

      if (!string.IsNullOrEmpty(connection))
        properties.Items.Add("connection", connection);

      return new ChallengeResult("Auth0", properties);
    }


    [Authorize]
    public IActionResult Profile()
    {
      return View(new UserProfileViewModel()
      {
        Name = User.Identity.Name,
        EmailAddress = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
        ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
      });
    }


    [Authorize(Roles = "admin")]
    public IActionResult Admin()
    {
      return View();
    }

  }
}





  
