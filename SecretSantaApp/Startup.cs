using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecretSantaApp.BL;
using SecretSantaApp.Models;

namespace SecretSantaApp
{
  public class Startup
  {
    //private IConfigurationRoot _configurationRoot;

    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();

    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

      services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


      services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();

      services.AddTransient<IGroupDal, GroupDal>();
      services.AddTransient<ICustomUserDal, CustomUserDal>();
      services.AddTransient<IGroupMembershipDal, GroupMembershipDal>();

      // Add authentication services
      services.AddAuthentication(
        options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);


      //Test Dependency Injection
      services.AddTransient<ISecretSantaBl, SecretSantaBl>();

      // Add framework services.
      services.AddMvc();

      //session and cache
      services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
      services.AddSession();

      // Add functionality to inject IOptions<T>
      services.AddOptions();

      // Add the Auth0 Settings object so it can be injected
      services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<Auth0Settings> auth0Settings)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      //app related settings
      app.UseStaticFiles();
      
      // IMPORTANT: This session call MUST go before UseMvc()
      app.UseSession();


      // Add the cookie middleware
      app.UseCookieAuthentication(new CookieAuthenticationOptions
      {
        LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/LoginExternal"),
        AutomaticAuthenticate = true,
        AutomaticChallenge = true
      });


      // Add the OIDC middleware
      var options = new OpenIdConnectOptions("Auth0")
      {
        // Set the authority to your Auth0 domain
        Authority = $"https://{auth0Settings.Value.Domain}",

        // Configure the Auth0 Client ID and Client Secret
        ClientId = auth0Settings.Value.ClientId,
        ClientSecret = auth0Settings.Value.ClientSecret,

        // Do not automatically authenticate and challenge
        AutomaticAuthenticate = false,
        AutomaticChallenge = false,

        // Set response type to code
        ResponseType = "code",

        // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
        CallbackPath = new PathString("/signin-auth0"),

        // Configure the Claims Issuer to be Auth0
        ClaimsIssuer = "Auth0",

        // Set the correct name claim type
        TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "name"
        },

        // Saves tokens to the AuthenticationProperties
        SaveTokens = true,


        Events = new OpenIdConnectEvents
        {
          // handle the logout redirection 
          OnRedirectToIdentityProviderForSignOut = (context) =>
          {
            var logoutUri = $"https://{auth0Settings.Value.Domain}/v2/logout?client_id={auth0Settings.Value.ClientId}";

            var postLogoutUri = context.Properties.RedirectUri;
            if (!string.IsNullOrEmpty(postLogoutUri))
            {
              if (postLogoutUri.StartsWith("/"))
              {
                // transform to absolute
                var request = context.Request;
                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
              }
              logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
            }

            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
          }
        }
      };
      options.Scope.Clear();
      options.Scope.Add("openid");
      options.Scope.Add("name");
      options.Scope.Add("email");
      options.Scope.Add("picture");
      options.Scope.Add("country");
      options.Scope.Add("roles");
      app.UseOpenIdConnectAuthentication(options);

      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
      
      
    }
  }
}
