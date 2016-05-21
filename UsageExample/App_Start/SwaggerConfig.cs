using ResxDescriptionFilters.UsageExample;
using Swashbuckle.Application;
using System.Web.Http;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace ResxDescriptionFilters.UsageExample
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
                    c.SchemaFilter(() => new ResourceSchemaFilter(ApiDocumentation.ResourceManager));
                    c.OperationFilter(() => new ResourceOperationFilter(ApiDocumentation.ResourceManager));
                })
                .EnableSwaggerUi();
        }
    }
}
