namespace Swashbuckle.DynamicLocalization
{
    using Swashbuckle.Application;
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web.Http;
    using System.Web.Http.Routing;

    public static class SwaggerEnabledConfigurationExtensions
    {
        private static readonly string DefaultRouteTemplate = "swagger/docs/{apiVersion}";

        public static void EnableLocalizedSwaggerUi(this SwaggerEnabledConfiguration configuration, Action<SwaggerUiConfig> configure = null)
        {
            EnableLocalizedSwaggerUi(configuration, DefaultRouteTemplate, configure);
        }

        public static void EnableLocalizedSwaggerUi(
            this SwaggerEnabledConfiguration configuration,
            string routeTemplate,
            Action<SwaggerUiConfig> configure = null)
        {
            var config = new SwaggerUiConfig(configuration.DiscoveryPaths, configuration.RootUrlResolver);
            if (configure != null) configure(config);

            var innerHandler = new SwaggerUiHandler(config);
            var handler = new StreamTransformHandler(innerHandler, s => new PlaceholderResolverStream(s, ResolvePlaceholder));

            configuration.HttpConfig.Routes.MapHttpRoute(
                name: "swagger_localizedui",
                routeTemplate: routeTemplate,
                defaults: null,
                constraints: new { assetPath = @".+" },
                handler: handler
            );

            if (routeTemplate == DefaultRouteTemplate)
            {
                configuration.HttpConfig.Routes.MapHttpRoute(
                    name: "swagger_localizedui_shortcut",
                    routeTemplate: "swagger",
                    defaults: null,
                    constraints: new { uriResolution = new HttpRouteDirectionConstraint(HttpRouteDirection.UriResolution) },
                    handler: new RedirectHandler(configuration.RootUrlResolver, "swagger/ui/index"));
            }
        }

        private static string ResolvePlaceholder(string key)
        {
            if (key == "%(TranslationScripts)")
            {
                string cultureName = CultureInfo.CurrentUICulture.Name;

                StringBuilder resultBuilder = new StringBuilder();
                resultBuilder.AppendLine("<script src='lang/translator-js' type='text/javascript'></script>");
                resultBuilder.AppendLine($"<script src='lang/{cultureName.Substring(0, 2)}-js' type='text/javascript'></script>");
                return resultBuilder.ToString();
            }
            else
            {
                // Leave unrecognised keys alone
                return key;
            }

            //// TODO: Also perhaps try resource string lookup to provide a method for localisation of non-SwaggerUI page content?
            //// A poor man's rendering engine..
        }
    }
}
