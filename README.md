# SwashbuckleResxDescriptionFilters

Swashbuckle filters that grab descriptions from resx files. Also includes some logic to expose the localisation functionality that Swagger UI includes.

## Usage - Filters

This package includes two Swashbuckle filter classes - ResourceOperationFilter and ResourceSchemaFilter which retrieve operation and schema descriptions from a provided ResourceManager, using the CurrentUICulture These can either be added separately using the usual method, or you can use the AddDynamicLocalisationFilters extension method for SwaggerDocsConfig to attach them both at once.

Resource look-up is convention based:
 
 * For operations, ...
 * For models, ...

It's up to you how you set CurrentUICulture. One way is with a handler for the ASP.NET BeginRequest event that sets it based on the first priority language header - as follows:

    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        string[] userLanguages = ((HttpApplication)sender).Request.UserLanguages;

        if (userLanguages != null && userLanguages.Length > 0)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(userLanguages[0]);
        }
    }

## Usage - Swagger UI localisation

Instead of EnableSwaggerUi, you can invoke the EnabledLocalizedSwaggerUI extension method provided by this package. Doing so will enable Swagger UI just as EnableSwaggerUi does, and have two additional effects:

 * A placeholder of %(TranslationScripts) in your index file will be resolved with the appropriate Swagger UI language scripts for the CurrentUICulture - as long as it is one for which Swagger UI language scripts exist. Note that this package does not provide an index file with this placeholder already in place. That's a TODO..
 * _If_ you provide a ResourceManager argument to EnableLocalizedSwaggerUi, a lookup (again using the CurrentUICulture) will be performed for any other placeholders (%(...)) in your index file in case there's anything else you want to localise.

## Implementation Woes

It's more difficult than anticipated to get the Swagger UI localisation working. Here's what I've tried:

Approach 1: Write an extension method for SwaggerEnabledConfiguration that creates the route I want - a handler that transforms the output wrapped around the SwaggerUIHandler. Nope - the configuration and .. fields are private, so I can't invoke the configuration callback with the right arguments.

Approach 2: Leave Swashbuckle's route creation untouched, and instead create another HTTP configuration extension method that modifies things - removes the Swagger UI route, replacing it with my version using the delegating handler. Nope - despite implementing interfaces that would suggest otherwise, routes can't be deleted. Okay so we leave it alone but insert one.. Also nope. Interestingly though, the routes _can_ be cleared. I'd be interested to hear the justification for this behaviour..

Approach 3: Give up.. For now. Ponder a Swashbuckle tweak and pull request so that approach 1 is workable..
