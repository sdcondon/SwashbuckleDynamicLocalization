namespace Swashbuckle.DynamicLocalization
{
    using Swashbuckle.Swagger;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Web.Http.Controllers;
    using System.Web.Http.Description;

    /// <summary>
    /// Operation filter that retrieves summaries and descriptions from a resource manager using the current UI culture.
    /// </summary>
    public class ResourceOperationFilter : IOperationFilter
    {
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceOperationFilter"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager from which to retrieve descriptions.</param>
        public ResourceOperationFilter(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <inheritdoc />
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var reflectedActionDescriptor = apiDescription.ActionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor == null)
            {
                return;
            }

            // TODO: Try just the type name first - use only as much of the full name as is needed?
            MethodInfo methodInfo = reflectedActionDescriptor.MethodInfo;
            string methodKey = methodInfo.DeclaringType.FullName + "." + methodInfo.Name;

            ApplyOperationComments(operation, methodKey);
            ApplyParamComments(operation, methodInfo, methodKey);
            ApplyResponseComments(operation, methodInfo, methodKey);
        }

        private void ApplyOperationComments(Operation operation, string methodKey)
        {
            string summary = _resourceManager.GetString(methodKey + ".Summary", CultureInfo.CurrentUICulture);
            if (summary != null)
            {
                operation.summary = summary;
            }

            string description = _resourceManager.GetString(methodKey + ".Description", CultureInfo.CurrentUICulture);
            if (description != null)
            {
                operation.description = description;
            }
        }

        private void ApplyParamComments(Operation operation, MethodInfo methodInfo, string methodKey)
        {
            if (operation.parameters == null)
            {
                return;
            }

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var parameter = operation.parameters.SingleOrDefault(param => param.name == parameterInfo.Name);
                if (parameter != null)
                {
                    parameter.description = _resourceManager.GetString(methodKey + "." + parameterInfo.Name, CultureInfo.CurrentUICulture);
                }
            }
        }

        private static void ApplyResponseComments(Operation operation, MethodInfo methodInfo, string methodKey)
        {
            //// TODO!
        }
    }
}