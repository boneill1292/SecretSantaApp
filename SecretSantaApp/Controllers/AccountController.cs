using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SecretSantaApp.Controllers
{
  /// <summary>
  /// For the Login, you will need to return a ChallengeResult and specify "Auth0" as the authentication scheme which will be challenged. This will invoke the OIDC middleware you registered in the Configure method.
  ///After the OIDC middleware has signed the user in, the user will automatically be signed into the cookie middleware as well to authenticate them on subsequent requests.So, for the Logout action you will need to sign the user out of both the OIDC and the cookie middleware:
  /// </summary>
  public class AccountController : Controller
  {
    public IActionResult Login()
    {
      return new ChallengeResult("Auth0", new AuthenticationProperties() { RedirectUri = "/" });
    }

    public async Task Logout()
    {
      await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
      {
        RedirectUri = Url.Action("Index", "Home")
      });
      await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
  }
}
