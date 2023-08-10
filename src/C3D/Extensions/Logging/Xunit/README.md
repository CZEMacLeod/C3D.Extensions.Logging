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

### Direct injection

```c#
private readonly ITestOutputHelper output;

ILogger<ClassUnderTest> log = output.CreateLogger<ClassUnderTest>();

\\ Assuming that the constuctor of ClassUnderTest takes an ILogger or ILogger<ClassUnderTest>
var sut = new ClassUnderTest(log);

\\ perform tests with logging...
```

Here the ILogger creation line might replace something like 
```c#
ILogger<ClassUnderTest> log = Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance.CreateLogger<ClassUnderTest>();
```

### Factory

You can also create a LoggerFactory if you need to using something like

```c#
private readonly ITestOutputHelper output;

var loggerFactory = LoggerFactory.Create(builder => builder.AddXunit(output));
```

### Fixtures

The same mechanisms used in tests based on injecting `ITestOutputHelper` into the test class, can be used in fixtures.
However, in fixtures you inject `IMessageSink`. The same extension methods are available for both interfaces.

