namespace Swashbuckle.DynamicLocalization
{
    using Swashbuckle.Application;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Routing;

    public static class SwaggerEnabledConfigurationExtensions
    {
        private static readonly string DefaultRouteTemplate = "swagger/ui/{*assetPath}";
        private static string[] languageScripts;

        public static void EnableLocalizedSwaggerUi(this SwaggerEnabledConfiguration configuration, Action<SwaggerUiConfig> configure = null)
        {
            EnableLocalizedSwaggerUi(configuration, DefaultRouteTemplate, null, configure);
        }

        public static void EnableLocalizedSwaggerUi(
            this SwaggerEnabledConfiguration configuration,
            string routeTemplate,
            Action<SwaggerUiConfig> configure = null)
        {
            EnableLocalizedSwaggerUi(configuration, routeTemplate, null, configure);
        }

        public static void EnableLocalizedSwaggerUi(
            this SwaggerEnabledConfiguration configuration,
            string routeTemplate,
            ResourceManager resourceManager,
            Action<SwaggerUiConfig> configure = null)
        {
            languageScripts = Assembly.GetAssembly(typeof(SwaggerEnabledConfiguration))
                .GetManifestResourceNames()
                .Where(a => a.StartsWith("lang\\"))
                .Select(a => a.Remove(0, 5))
                .ToArray();

            // Dirty, fragile, needless CAS-requiring hack - really would like these to be public properties: 
            var httpConfig = (HttpConfiguration)typeof(SwaggerEnabledConfiguration).GetField("_httpConfig", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(configuration);
            var discoveryPaths = (IEnumerable<string>)typeof(SwaggerEnabledConfiguration).GetField("_discoveryPaths", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(configuration);
            var rootUrlResolver = (Func<HttpRequestMessage, string>)typeof(SwaggerEnabledConfiguration).GetField("_rootUrlResolver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(configuration);

            var config = new SwaggerUiConfig(discoveryPaths, rootUrlResolver);
            configure?.Invoke(config);

            var innerHandler = new SwaggerUiHandler(config);
            var transformHandler = new HtmlStreamTransformHandler(innerHandler, s => new PlaceholderResolverStream(s, a => ResolvePlaceholder(resourceManager, a)));

            httpConfig.Routes.MapHttpRoute(
                name: "swagger_localizedui",
                routeTemplate: routeTemplate,
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: transformHandler);

            if (routeTemplate == DefaultRouteTemplate)
            {
                httpConfig.Routes.MapHttpRoute(
                    name: "swagger_localizedui_shortcut",
                    routeTemplate: "swagger",
                    defaults: null,
                    constraints: new { uriResolution = new HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                    handler: new RedirectHandler(rootUrlResolver, "swagger/ui/index"));
            }
        }

        private static string ResolvePlaceholder(ResourceManager resourceManager, string key)
        {
            if (key == "%(TranslationScripts)")
            {
                StringBuilder resultBuilder = new StringBuilder();

                string cultureName = CultureInfo.CurrentUICulture.Name;
                                
                if ((cultureName = GetSwaggerUiLanguageFile(cultureName)) != null)
                {
                    resultBuilder.AppendLine("<script src='lang/translator-js' type='text/javascript'></script>");
                    resultBuilder.AppendLine($"<script src='lang/{cultureName}-js' type='text/javascript'></script>");
                }

                return resultBuilder.ToString();
            }
            else
            {
                // Try resource string lookup to provide a method for localisation of non-SwaggerUI page content - a poor man's rendering engine..
                // Finally, we leave unrecognised keys alone - slim chance it might not have been intended as a placeholder
                string resourceStringValue = resourceManager?.GetString(key.Substring(2, key.Length - 3));
                if (resourceStringValue != null)
                {
                    return resourceStringValue;
                }
                else
                {
                    return key;
                }
            }
        }

        private static string GetSwaggerUiLanguageFile(string cultureName)
        {
            if (languageScripts.Contains(cultureName + ".js"))
            {
                return cultureName;
            }
            else if (languageScripts.Contains(cultureName.Substring(0, 2) + ".js"))
            {
                return cultureName.Substring(0, 2);
            }
            else
            {
                return null;
            }
        }
    }
}
