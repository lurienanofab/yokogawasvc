/*
   Copyright 2017 University of Michigan

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License. 
*/

using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Swashbuckle.Application;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(YokogawaService.Startup))]

namespace YokogawaService
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            app.Use(async (ctx, next) =>
            {
                using (var uow = DataManager.Current.StartUnitOfWork())
                {
                    ctx.Set("uow", uow);
                    await next.Invoke();
                }
            });

            var corsPolicy = new EnableCorsAttribute(
                ConfigurationManager.AppSettings["cors:Origins"],
                ConfigurationManager.AppSettings["cors:Headers"],
                ConfigurationManager.AppSettings["cors:Methods"]);

            config
                .EnableCors(corsPolicy);

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "YokogawaService API"))
                .EnableSwaggerUi();

            config.EnsureInitialized();

            app.UseWebApi(config);
        }
    }

    public static class HttpRequestMessageExtensions
    {
        public static UnitOfWork GetUnitOfWork(this HttpRequestMessage request)
        {
            return request.GetOwinContext().Get<UnitOfWork>("uow");
        }
    }
}
