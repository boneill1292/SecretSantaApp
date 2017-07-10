using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretSantaApp.BL;

namespace SecretSantaApp.Controllers
{
  public class QuickPickController : Controller
  {
    private readonly ISecretSantaBl _secretSantaBl;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GroupsController> _log;


    public QuickPickController(ISecretSantaBl secretSantaBl, IHttpContextAccessor httpContextAccessor,
      ILogger<GroupsController> log)
    {
      _secretSantaBl = secretSantaBl;
      _httpContextAccessor = httpContextAccessor;
      _log = log;
    }


    public ActionResult Index()
    {
      return View("QuickPick");
    }
  }
}
