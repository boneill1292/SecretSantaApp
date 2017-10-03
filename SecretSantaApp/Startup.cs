using System;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecretSantaApp.BL;
using SecretSantaApp.DAL;
using SecretSantaApp.Models;
using SecretSantaApp.Services;

namespace SecretSantaApp
{
    public class Startup
    {
        //private IConfigurationRoot _configurationRoot;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                // .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            //Data Access Layer Services
            services.AddTransient<IGroupDal, GroupDal>();
            services.AddTransient<ICustomUserDal, CustomUserDal>();
            services.AddTransient<IGroupMembershipDal, GroupMembershipDal>();
            services.AddTransient<IGroupRulesDal, GroupRulesDal>();
            services.AddTransient<IGroupMessagesDal, GroupMessagesDal>();
            services.AddTransient<IMemberConditionsDal, MemberConditionsDal>();
            services.AddTransient<ICustomUserDetailsDal, CustomUserDetailsDal>();
            services.AddTransient<IGroupPairingsDal, GroupPairingsDal>();


            //Test Dependency Injection
            services.AddTransient<ISecretSantaBl, SecretSantaBl>();
            services.AddTransient<IViewRenderService, ViewRenderService>();


            // Add authentication services
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect("Auth0", options =>
                {
                    // Set the authority to your Auth0 domain
                    options.Authority = $"https://{Configuration["Auth0:Domain"]}";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = Configuration["Auth0:ClientId"];
                    options.ClientSecret = Configuration["Auth0:ClientSecret"];


                    // Set response type to code
                    options.ResponseType = "code";

                    // Configure the scope
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");

                    // Set the correct name claim type
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name"
                    };


                    // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0 
                    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard 
                    options.CallbackPath = new PathString("/signin-auth0");

                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Auth0";

                    options.Events = new OpenIdConnectEvents
                    {
                        // handle the logout redirection 
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            var logoutUri =
                                $"https://{Configuration["Auth0:Domain"]}/v2/logout?client_id={Configuration["Auth0:ClientId"]}";

                            var postLogoutUri = context.Properties.RedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = context.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase +
                                                    postLogoutUri;
                                }
                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            context.Response.Redirect(logoutUri);
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });


            // Add framework services.
            services.AddMvc();


            services.AddSingleton<IConfiguration>(Configuration);

            //var testtest = Configuration["TestMe:TestOne"];
            //var testthree = Configuration["TestMe:TestTwo"];
            //options.ClientId = Configuration["Auth0:ClientId"];
            //options.ClientSecret = Configuration["Auth0:ClientSecret"];


            //session and cache
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            //MailGun
            var domain = Configuration["MailGun:Domain"];
            var mgkey = Configuration["MailGun:ApiKey"];
            var sender = new MailgunSender(
                domain, // Mailgun Domain
                mgkey // Mailgun API Key
            );
            Email.DefaultSender = sender;


            //auth0
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<Auth0Settings> auth0Settings)
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
                //app.UseDeveloperExceptionPage();
            }

            // loggerFactory.AddFile("Logs/SecretSantaApp-{Date}.txt");

            //app related settings
            app.UseStaticFiles();

            // IMPORTANT: This session call MUST go before UseMvc()
            app.UseSession();

            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}