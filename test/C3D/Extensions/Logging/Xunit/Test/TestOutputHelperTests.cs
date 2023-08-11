using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using C3D.Extensions.Logging.Xunit.Utilities;
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

    [Fact]
    public void CreateLogger()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TestOutputHelperTests>();
        
        Assert.NotNull(log);
        
        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreateLoggerWithTimestamp()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TestOutputHelperTests>(configure=>configure.TimeStamp= XunitLoggerTimeStamp.DateTime);

        Assert.NotNull(log);

        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreateLoggerWithOffset()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TestOutputHelperTests>(configure => configure.TimeStamp = XunitLoggerTimeStamp.Offset);

        Assert.NotNull(log);

        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreateLoggerFiltersMessages()
    {
        WriteFunctionName();

        var wrapper = new LoggingTestOutputHelper(output);

        var log = wrapper.CreateLogger<TestOutputHelperTests>(configure => configure.MinLevel = LogLevel.Information);

        Assert.NotNull(log);

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

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

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

        log.LogDebug(debug);
        log.LogInformation(info);

        var wrapper = services.GetRequiredService<ITestOutputHelper>() as LoggingTestOutputHelper;

        Assert.NotNull(wrapper);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s=>s.EndsWith(debug), s => s.EndsWith(info));
    }

    [Fact]
    public void CreateLoggerFromFactory()
    {
        WriteFunctionName();

        var (wrapper, factory) = CreateLoggerFactory();

        var log = factory.CreateLogger<TestOutputHelperTests>();

        Assert.NotNull(log);

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s => s.EndsWith(debug), s => s.EndsWith(info));
    }


}