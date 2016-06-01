namespace Swashbuckle.DynamicLocalization
{
    using Swashbuckle.Swagger;
    using System;
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Implementation of <see cref="ISchemaFilter"/> that retrieves model descriptions from a given resource manager,
    /// using the current UI culture. The description for a model is obtained from the resource named for its full type
    /// name. The key for the description for a property is a full type name with a period and the property name appended.
    /// </summary>
    public class ResourceSchemaFilter : ISchemaFilter
    {
        private ResourceManager _resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSchemaFilter"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager from which to retrieve descriptions.</param>
        public ResourceSchemaFilter(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <inheritdoc />
        public void Apply(Schema model, SchemaRegistry schemaRegistry, Type type)
        {
            // TODO: Try just the type name first - use only as much of the full name as is needed?
            string modelKey = type.FullName;
            model.description = _resourceManager.GetString(modelKey, CultureInfo.CurrentUICulture);

            foreach (var modelProperty in model.properties)
            {
                var propertyInfo = type.GetProperty(modelProperty.Key);
                if (propertyInfo != null)
                {
                    string resourceName = modelKey + "." + propertyInfo.Name;
                    modelProperty.Value.description = _resourceManager.GetString(resourceName, CultureInfo.CurrentUICulture);
                }
            }
        }
    }
}