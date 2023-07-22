# C3D.Extensions.Logging.Xunit

An implementation of `Microsoft.Extensions.Logging` Loggers wrapping xunit output types.
This allows you to capture logging output when using `dotnet test` etc.
It can be injected into services under test, or used as a provider in DI.

It supports some minor configuration which can be added via the normal configuration sources, such as appsettings.json, usersecrets etc.

## Configuration
```json
{
	"Logging": {
		"Xunit": {
			"MinLevel": "Trace",
			"UseOffset": true,
			"LogLevel": {
				"Default": "Trace"
			}
		}
	}
}
```

## Usage

### Dependency Injection

```c#
services
	.AddSingleton(output)
	.AddLogging(logging => logging
		.ClearProviders()
		.SetMinimumLevel(LogLevel.Debug)
		.AddXunit());
```

Here `output` can be either `ITestOutputHelper` or `IMessageSink`, depending on whether you are using this from a unit test, or a class fixture.

Alternatively, you can explicitly add the output, and/or setup the logging configuration inside the `AddXunit` extension method.

```c#
logging
	.AddXunit(output, config=>{
	});
```