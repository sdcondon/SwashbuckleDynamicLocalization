﻿using Swashbuckle.Swagger;
using System;
using System.Globalization;
using System.Resources;

namespace ResxDescriptionFilters
{
    /// <summary>
    /// Implementation of <see cref="ISchemaFilter"/> that sets model descriptions using a given resource manager.
    /// The description for a model is obtained from the resource named for its full type name. The key for the
    /// description for a property is a full type name with a period and the property name appended.
    /// The current UI culture is used for the lookup.
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
            // TODO: Try just the type name first - only use as much of the full name as is needed?
            string modelKey = type.FullName.Replace('.', '_');
            model.description = _resourceManager.GetString(modelKey, CultureInfo.CurrentUICulture);

            foreach (var modelProperty in model.properties)
            {
                var propertyInfo = type.GetProperty(modelProperty.Key);
                if (propertyInfo != null)
                {
                    string resourceName = modelKey + "_" + propertyInfo.Name;
                    modelProperty.Value.description = _resourceManager.GetString(resourceName, CultureInfo.CurrentUICulture);
                }
            }
        }
    }
}