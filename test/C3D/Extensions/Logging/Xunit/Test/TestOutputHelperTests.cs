using C3D.Extensions.Logging.Xunit.Utilities;
using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

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
    public void CreateLoggerWithConfigure()
    {
        WriteFunctionName();

        var utcNow = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
        var tp = new FakeTimeProvider(utcNow);
        var wrapper = new LoggingTestOutputHelper(output);

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure =>
        {
            configure
                .UseTimeProvider(tp)
                .WithTimeStamp(XunitLoggerTimeStamp.Offset)
                .UseCulture(System.Globalization.CultureInfo.InvariantCulture)
                .MinLevel = LogLevel.Information;
        });

        Assert.NotNull(log);

        log.LogInformation(info);

        Assert.True(log.IsEnabled(LogLevel.Information));

        Assert.False(log.IsEnabled(LogLevel.Debug));

        const string formattedString = "| 0:00:00:00.0000000 C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information";
        var messages = wrapper.Messages;
        Assert.Single(messages, formattedString);
    }

    [Fact]
    public void CreateLoggerWithLevel()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);

#pragma warning disable CS0618 // Type or member is obsolete
        var log = wrapper.CreateLogger<TestOutputHelperTests>(LogLevel.Information);
#pragma warning restore CS0618 // Type or member is obsolete

        Assert.NotNull(log);

        log.LogInformation(info);

        Assert.True(log.IsEnabled(LogLevel.Information));

        Assert.False(log.IsEnabled(LogLevel.Debug));

        const string formattedString = "| C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information";
        var messages = wrapper.Messages;
        Assert.Single(messages, formattedString);
    }

    [Fact]
    public void CreateLoggerWithLevelAndStart()
    {
        WriteFunctionName();

        var utcNow = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
        var tp = new FakeTimeProvider(utcNow);
        var wrapper = new LoggingTestOutputHelper(output);

#pragma warning disable CS0618 // Type or member is obsolete
        // If you provide a start time, it assumes offset format,
        // but we can't provide a timeprovider here
        // so it is difficult to mock/test.
        var log = wrapper.CreateLogger<TestOutputHelperTests>(LogLevel.Information, utcNow);
#pragma warning restore CS0618 // Type or member is obsolete

        Assert.NotNull(log);

        log.LogInformation(info);

        Assert.True(log.IsEnabled(LogLevel.Information));

        Assert.False(log.IsEnabled(LogLevel.Debug));

        const string formattedString = "C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information";
        var messages = wrapper.Messages;
        Assert.Single(messages, m=> m.EndsWith(formattedString));
    }


    [Fact]
    public void CreateLoggerWithOptions()
    {
        WriteFunctionName();

        var utcNow = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
        var tp = new FakeTimeProvider(utcNow);
        var wrapper = new LoggingTestOutputHelper(output);

        var options = new XunitLoggerOptions()
            .UseTimeProvider(tp)
            .WithTimeStamp(XunitLoggerTimeStamp.Offset)
            .UseCulture(System.Globalization.CultureInfo.InvariantCulture);
        options.MinLevel = LogLevel.Information;
        var log = wrapper.CreateLogger<TestOutputHelperTests>(options);

        Assert.NotNull(log);

        log.LogInformation(info);

        Assert.True(log.IsEnabled(LogLevel.Information));

        Assert.False(log.IsEnabled(LogLevel.Debug));

        const string formattedString = "| 0:00:00:00.0000000 C3D.Extensions.Logging.Xunit.Test.TestOutputHelperTests Information | Here is some information";
        var messages = wrapper.Messages;
        Assert.Single(messages, formattedString);
    }

    [Fact]
    public void CreateLoggerWithTimestamp()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);
        var utcNow = new DateTimeOffset(2020, 10, 9, 8, 7, 6, TimeSpan.Zero);
        var tp = new FakeTimeProvider(utcNow);

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure
            .UseTimeProvider(tp)
            .WithTimeStamp(XunitLoggerTimeStamp.DateTime)
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

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure
            .UseTimeProvider(tp)
            .WithTimeStamp(XunitLoggerTimeStamp.Offset)
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

        Assert.Single(messages, s => s.EndsWith(info));
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