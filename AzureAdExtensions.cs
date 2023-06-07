using EPiServer.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace optimizely_cms12_azure_ad
{
    public static class AzureAdExtensions
    {
        public static IServiceCollection AddAzureAd(this IServiceCollection services, IConfiguration configuration)
        {
            string cookie = ".Azure." + CookieAuthenticationDefaults.AuthenticationScheme;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = cookie;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddCookie(cookie, options =>
           {
               options.Cookie.Name = cookie;
               options.Events.OnSignedIn = async ctx =>
               {
                   var identity = ctx.Principal?.Identity;
                   if (identity is ClaimsIdentity claimsIdentity)
                   {
                       // Syncs user and roles so they are available to the CMS
                       var synchronizingUserService = ctx
                           .HttpContext
                           .RequestServices
                           .GetRequiredService<ISynchronizingUserService>();

                       await synchronizingUserService.SynchronizeAsync(claimsIdentity);
                   }
               };
           })
           .AddOpenIdConnect(options =>
           {
               options.SignInScheme = cookie;
               options.ResponseType = OpenIdConnectResponseType.Code;
               options.UsePkce = true;
               options.Authority = "https://login.microsoftonline.com/" + configuration["TenantId"] + "/v2.0";
               options.ClientId = configuration["ClientId"];
               options.ClientSecret = configuration["ClientSecret"];

               options.Scope.Clear();
               options.Scope.Add(OpenIdConnectScope.OpenId);
               options.Scope.Add(OpenIdConnectScope.OfflineAccess);
               options.Scope.Add(OpenIdConnectScope.Email);
               options.MapInboundClaims = false;

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   RoleClaimType = "roles",
                   NameClaimType = "email",
                   ValidateIssuer = false
               };


               options.Events.OnTokenValidated = context =>
               {
                   // enable to print claims debug information in the console
                   //var user = context.Principal.Identity;

                   //if (user != null)
                   //{
                   //    if (user is ClaimsIdentity claimsIdentity)
                   //    {
                   //        claimsIdentity.AddClaim(new Claim("roles", "WebAdmins"));
                   //        context.Principal.AddIdentity(claimsIdentity);
                   //    }

                   //    //var claims = ((System.Security.Claims.ClaimsIdentity)context.Principal.Identity).Claims;

                   //    //foreach (var claim in claims)
                   //    //{
                   //    //    Console.WriteLine($"{claim.Type}: {claim.Value}");
                   //    //}
                   //}
                   return Task.FromResult(0);

               };

               options.Events.OnRedirectToIdentityProvider = ctx =>
               {
                   // Prevent redirect loop
                   if (ctx.Response.StatusCode == 401)
                   {
                       ctx.HandleResponse();
                   }

                   return Task.CompletedTask;
               };

               options.Events.OnAuthenticationFailed = context =>
               {
                   context.HandleResponse();
                   context.Response.BodyWriter.WriteAsync(Encoding.ASCII.GetBytes(context.Exception.Message));
                   return Task.CompletedTask;
               };

           });

            return services;
        }
    }
}
