# SwashbuckleResxDescriptionFilters

Swashbuckle filters that grab descriptions from resx files. Also includes some logic to expose the localisation functionality that Swagger UI includes.

## Usage - Resx filters

TODO: explain the filters

## Usage - Swagger UI localisation

TODO: Explain

## Implementation Woes

For the interested. It was far more difficult than anticipated to get the Swagger UI localisation working. Here's what I tried:

Approach 1: Write an extension method for SwaggerEnabledConfiguration that creates the route I want - a handler that transforms the output wrapped around the SwaggerUIHandler. Nope - the configuration and .. fields are private, so I can't invoke the configuration callback with the right arguments.

Approach 2: Leave Swashbuckle's route creation untouched, and instead create another HTTP configuration extension method that modifies things - removes the Swagger UI route, replacing it with my version using the delegating handler. Nope - despite implementing interfaces that would suggest otherwise, routes can't be deleted. Okay so we leave it alone but insert one.. Also nope. Interestingly though, the routes _can_ be cleared. I'd be interested to hear the justification for this behaviour..

Approach 3: Give up.. For now. Ponder a Swashbuckle tweak and pull request so that approach 1 is workable..
