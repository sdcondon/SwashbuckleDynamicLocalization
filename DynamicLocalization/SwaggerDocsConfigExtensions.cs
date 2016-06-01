namespace Swashbuckle.DynamicLocalization
{
    using Swashbuckle.Application;
    using System.Resources;

    /// <summary>
    /// Extension methods for the <see cref="SwaggerDocsConfig"/> class.
    /// </summary>
    public static class SwaggerDocsConfigExtensions
    {
        public static void AddDynamicLocalizationFilters(this SwaggerDocsConfig config, ResourceManager resourceManager)
        {
            config.SchemaFilter(() => new ResourceSchemaFilter(resourceManager));
            config.OperationFilter(() => new ResourceOperationFilter(resourceManager));
        }
    }
}
