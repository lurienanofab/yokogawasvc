using Microsoft.Owin;
using Owin;
using Swashbuckle.Application;
using System.Configuration;
using System.Net.Http;
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
