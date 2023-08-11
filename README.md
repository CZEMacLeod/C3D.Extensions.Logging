# C3D.Extensions.Logging

A set of Logging related packages, most that are based on `Microsoft.Extensions.Logging`

## C3D.Extensions.Logging.Xunit

[![NuGet package](https://img.shields.io/nuget/v/C3D.Extensions.Logging.Xunit.svg)](https://nuget.org/packages/C3D.Extensions.Logging.Xunit)
[![NuGet downloads](https://img.shields.io/nuget/dt/C3D.Extensions.Logging.Xunit.svg)](https://nuget.org/packages/C3D.Extensions.Logging.Xunit)

An implementation of `Microsoft.Extensions.Logging` Loggers wrapping the `xunit` 'output' types `ITestOutputHelper` and `IMessageSink`.
This allows you to take the logged output from things like hosting or SUT classes that take an `ILogger` and show it in the test output.


