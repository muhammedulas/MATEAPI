using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Security.Claims;
using System.Linq;
using MATE.viewModels;
using System.Collections.Generic;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(MATE.App_Start.Owin_Startup))]

namespace MATE.App_Start
{
    public class Owin_Startup


    {
        
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new Microsoft.Owin.PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true,
                Provider = new SimpleAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }

    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        readonly MATEEntities db = new MATEEntities();
        readonly Md5Crypt hashTool = new Md5Crypt();
        private const string apiKey = "aeccc56501920c9c7a60391d8932c0c83c1d1cca4f98f8861f879d53a43acd19";

#pragma warning disable CS1998 // Zaman uyumsuz yöntemde 'await' işleçleri yok ve zaman uyumlu çalışacak
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
#pragma warning restore CS1998 // Zaman uyumsuz yöntemde 'await' işleçleri yok ve zaman uyumlu çalışacak
        {
                context.Validated();

           
        }

        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            userModel user = (userModel)db.USERS.Where(u => u.USERNAME == context.UserName).Select(u => new userModel()
            {
                USERID = u.USERID,
                USERNAME = u.USERNAME,
                PW_KEY = u.PW_KEY,
                NAME = u.NAME,
                SURNAME = u.SURNAME,
                IS_ADMIN = u.IS_ADMIN,
                STATUS = u.STATUS,

            }).SingleOrDefault();

            if (user == null)
            {
                context.SetError("invalid_grant","Kullanıcı Bulunamadı");
                return base.GrantResourceOwnerCredentials(context);
            };

            if (context.UserName == user.USERNAME && hashTool.CalculateHash(context.Password) == user.PW_KEY && user.STATUS == 1)
            {
                
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("sub", context.UserName));
                identity.AddClaim(new Claim("id", user.USERID.ToString()));

                var additionalInfo = new AuthenticationProperties();
                additionalInfo.Dictionary.Add("success", "true");
                additionalInfo.Dictionary.Add("userId", user.USERID.ToString());
                additionalInfo.Dictionary.Add("userName", user.USERNAME);
                additionalInfo.Dictionary.Add("name", user.NAME);
                additionalInfo.Dictionary.Add("surname", user.SURNAME);
                if (user.IS_ADMIN == true)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                    identity.AddClaim(new Claim("isAdmin", "true"));
                    additionalInfo.Dictionary.Add("isAdmin", "true");
                }
                else
                {
                    identity.AddClaim(new Claim("isAdmin", "false"));
                    additionalInfo.Dictionary.Add("isAdmin", "false");
                    if (db.USER_ROLES.Count(q=> q.USERREF == user.USERID && q.ROLEREF == 2) > 0)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                    }
                    else if (db.USER_ROLES.Count(q => q.USERREF == user.USERID && q.ROLEREF == 3) > 0)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "depManager"));
                    }
                    else if (db.USER_ROLES.Count(q => q.USERREF == user.USERID && q.ROLEREF == 4) > 0)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "compManager"));
                    }
                }
                var ticket = new AuthenticationTicket(identity, additionalInfo);

                context.Validated(ticket);

            }
            else
            {
                context.SetError("invalid_grant", "Kullanıcı adı veya parola hatalı");
            };
            
            return base.GrantResourceOwnerCredentials(context);


          
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}
