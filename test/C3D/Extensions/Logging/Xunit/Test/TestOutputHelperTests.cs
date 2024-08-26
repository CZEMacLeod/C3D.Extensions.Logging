using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using C3D.Extensions.Logging.Xunit.Utilities;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace C3D.Extensions.Logging.Xunit.Test;

public partial class TestOutputHelperTests
{
    private readonly ITestOutputHelper output;

    public TestOutputHelperTests(ITestOutputHelper output) => this.output = output;

    private void WriteFunctionName([CallerMemberName] string? caller = null) => output.WriteLine(caller);

    private static IConfiguration CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        return builder.Build();

    }

    private IServiceCollection CreateServices()
    {
        var configuration = CreateConfiguration();

        return new ServiceCollection()
                        .AddSingleton(configuration)
                        .AddSingleton<ITestOutputHelper>(new LoggingTestOutputHelper(output))
                        .AddLogging(logging => logging
                            .ClearProviders()
                            .SetMinimumLevel(LogLevel.Debug)
                            .AddConfiguration(configuration.GetSection("Logging"))
                            .AddDebug()
                            .AddXunit()
                            );
    }

    private (LoggingTestOutputHelper wrapper, ILoggerFactory factory) CreateLoggerFactory()
    {
        var configuration = CreateConfiguration();
        var wrapper = new LoggingTestOutputHelper(output);
        var factory = LoggerFactory.Create(logging => logging
                            .ClearProviders()
                            .SetMinimumLevel(LogLevel.Debug)
                            .AddConfiguration(configuration.GetSection("Logging"))
                            .AddDebug()
                            .AddXunit(wrapper));
        return (wrapper, factory);
    }

    const string debug = "Here is some debugging";
    const string info = "Here is some information";

    [Fact]
    public void CreateLogger()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TestOutputHelperTests>();

        Assert.NotNull(log);

        log.LogInformation(info);
    }

    [Fact]
    public void CreateLoggerWithTimestamp()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);
        var utcNow = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
        var tp = new FakeTimeProvider(utcNow);

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure
            .WithTimeStamp(XunitLoggerTimeStamp.DateTime)
            .UseTimeProvider(tp)
            .UseCulture(System.Globalization.CultureInfo.InvariantCulture)
            );

        Assert.NotNull(log);

        log.LogInformation(info);

        var messages = wrapper.Messages;

        var s = Assert.Single(messages);
        Assert.Equal("| 2020-10-09 08:07:06Z C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information", s);
    }

    [Fact]
    public void CreateLoggerWithOffset()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);
        var utcNow = DateTime.UtcNow;
        var tp = new FakeTimeProvider(utcNow);

        // "| -0:00:00:00.0007472 C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information"


        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure
            .WithTimeStamp(XunitLoggerTimeStamp.Offset)
            .UseTimeProvider(tp)
            .Restart()
            .UseCulture(System.Globalization.CultureInfo.InvariantCulture)
            );

        Assert.NotNull(log);

        log.LogInformation(info);

        var messages = wrapper.Messages;
        var s = Assert.Single(messages);
        Assert.Equal("| 0:00:00:00.0000000 C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information", s);
    }

    [Fact]
    public void CreateLoggerFiltersMessages()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure.MinLevel = LogLevel.Information);

        Assert.NotNull(log);

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s => s.EndsWith(info));
    }

    [Fact]
    public void CreateLoggerFromServices()
    {
        WriteFunctionName();

        var services = CreateServices().BuildServiceProvider();

        var log = services.GetRequiredService<ILogger<TestOutputHelperTests>>();

        Assert.NotNull(log);

        log.LogDebug(debug);
        log.LogInformation(info);

        var wrapper = services.GetRequiredService<ITestOutputHelper>() as LoggingTestOutputHelper;

        Assert.NotNull(wrapper);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s => s.EndsWith(debug), s => s.EndsWith(info));
    }

    [Fact]
    public void CreateLoggerFromFactory()
    {
        WriteFunctionName();

        var (wrapper, factory) = CreateLoggerFactory();

        var log = factory.CreateLogger<TestOutputHelperTests>();

        Assert.NotNull(log);

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s => s.EndsWith(debug), s => s.EndsWith(info));
    }


}