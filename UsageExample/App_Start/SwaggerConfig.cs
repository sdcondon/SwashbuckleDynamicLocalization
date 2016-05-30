using Swashbuckle.Application;
using Swashbuckle.DynamicLocalization.UsageExample;
using System.Reflection;
using System.Web.Http;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Swashbuckle.DynamicLocalization.UsageExample
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Swashbuckle.ResxDescriptionFilters.UsageExample");
                    //c.AddDynamicLocalisationFilters(ApiDocumentation.ResourceManager);
                })
                .EnableSwaggerUi(c =>
                {
                    c.CustomAsset("index", Assembly.GetExecutingAssembly(), "ResxDescriptionFilters.UsageExample.SwaggerUI.html");
                });

            //GlobalConfiguration.Configuration.EnableSwaggerUiLocalisation();
        }
    }
}
