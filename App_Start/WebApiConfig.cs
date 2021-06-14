using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using JWT;
using Microsoft.AspNetCore.Cors;

namespace MATE
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API yapılandırması ve hizmetleri
                       
            // Web API yolları
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            
        }

        //private static void EnableCrossSiteRequests(HttpConfiguration config)
        //{
        //    var cors = new EnableCorsAttribute(
        //        origins: "*",
        //        headers: "*",
        //        methods: "*");
        //    config.EnableCors(cors);
        //}
    }
}
