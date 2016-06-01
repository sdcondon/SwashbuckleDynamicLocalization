# Swashbuckle.DynamicLocalization

Two distinct pieces of functionality, extending on Swashbuckle:
 * Swashbuckle filters that grab descriptions from resource strings, using the CurrentUICulture.
 * Logic to expose the localisation functionality that Swagger UI includes, again using CurrentUICulture.

## Usage - Filters

This package includes two Swashbuckle filter classes - ResourceOperationFilter and ResourceSchemaFilter. These retrieve operation and schema descriptions from a provided ResourceManager, using the CurrentUICulture. They can either be added separately in the usual way, or you can use the AddDynamicLocalisationFilters extension method for SwaggerDocsConfig to attach them both at once.

Resource look-up is convention based the keys are as follows:
 
 * For an operation description: {namespace-qualified type name}.{method name}.Description
 * For an operation summary: {namespace-qualified type name}.{method name}.Summary
 * For an operation parameter description: {namespace-qualified type name}.{method name}.{parameter name}
 * For a model description: {namespace-qualified type name}
 * For a model property description: {namespace-qualified type name}.{property name}

(TODO: Only require as much of the namespace as needed to be unique)

It's up to you how you set CurrentUICulture. One way is with a handler for the ASP.NET BeginRequest event that sets it based on the first priority language header - as follows:

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        string[] userLanguages = ((HttpApplication)sender).Request.UserLanguages;

        if (userLanguages != null && userLanguages.Length > 0)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(userLanguages[0]);
        }
    }

See the usage example project for an example of this.

## Usage - Swagger UI localisation

Instead of EnableSwaggerUi, you can invoke the EnabledLocalizedSwaggerUI extension method provided by this package. Doing so will enable Swagger UI just as EnableSwaggerUi does, and have two additional effects:

 * A placeholder of %(TranslationScripts) in your index file will be resolved to script tags for the appropriate Swagger UI language scripts given the CurrentUICulture (as long as it is one for which Swagger UI language scripts exist - if not, no script tags will be added). Note that this package _does not_ provide an index file with this placeholder already in place. That's a TODO..
 * _If_ you provide a ResourceManager argument to EnableLocalizedSwaggerUi, a lookup (again using the CurrentUICulture) will be performed for any other placeholders (%(...)) in your index file in case there's anything else you want to localise.

## Feature List

 [ ] Submit a very polite pull request for Swashbuckle (once I figure out how to do that..) so that this package can depend on Swashbuckle.Core rather than including its own hideous mutation thereof..
 [ ] Documentation Filter (and might need some more SwaggerDocsConfig extensions) for localising other swagger doc entries - the title, for example.
 [ ] For the resource lookups in the operation and schema filters, only require as much of the namespace as is necessary to be unique.
 [ ] When EnableLocalizedSwaggerUi is invoked, register an index HTML file that's got the %(TranslationScripts) placeholder in
 [ ] Examine whether creating something equivalent for Ahoy would be useful..

## Implementation Woes

It's more difficult than anticipated to get the Swagger UI localisation working. Here's what I've tried:

Approach 1: Write an extension method for SwaggerEnabledConfiguration that creates the route I want - a handler that transforms the output wrapped around the SwaggerUIHandler. Nope - the configuration and .. fields are private, so I can't invoke the configuration callback with the right arguments.

Approach 2: Leave Swashbuckle's route creation untouched, and instead create another HTTP configuration extension method that modifies things - removes the Swagger UI route, replacing it with my version using the delegating handler. Nope - despite implementing interfaces that would suggest otherwise, routes can't be deleted. Okay so we leave it alone but insert one.. Also nope. Interestingly though, the routes _can_ be cleared. I'd be interested to hear the justification for this behaviour..

Approach 3: Give up.. For now. Ponder a Swashbuckle tweak and pull request so that approach 1 is workable..
