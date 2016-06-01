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
                    c.SingleApiVersion("v1", "Swashbuckle Dynamic Localization Usage Example");
                    c.AddDynamicLocalizationFilters(ApiDocumentation.ResourceManager);
                })
                .EnableLocalizedSwaggerUi(c =>
                {
                    c.CustomAsset("index", Assembly.GetExecutingAssembly(), "Swashbuckle.DynamicLocalization.UsageExample.SwaggerUI.html");
                });
        }
    }
}
