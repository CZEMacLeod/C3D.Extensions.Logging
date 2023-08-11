using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace C3D.Extensions.Logging.Xunit.Test;

public class TextOutputTest
{
    private readonly ITestOutputHelper output;

    public TextOutputTest(ITestOutputHelper output) => this.output = output;

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
                        .AddSingleton(output)
                        .AddLogging(logging => logging
                            .ClearProviders()
                            .SetMinimumLevel(LogLevel.Debug)
                            .AddConfiguration(configuration.GetSection("Logging"))
                            .AddXunit()
                            );
    }

    private class WrappedOutput : ITestOutputHelper
    {
        private readonly ITestOutputHelper output;
        private readonly List<string> messages = new();

        public IReadOnlyList<string> Messages => messages.ToImmutableList();

        public WrappedOutput(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void WriteLine(string message)
        {
            output.WriteLine(message);
            messages.Add(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            output.WriteLine(format, args);
            messages.Add(string.Format(format, args));
        }
    }

    [Fact]
    public void CreateLogger()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TextOutputTest>();
        
        Assert.NotNull(log);
        
        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreateLoggerWithTimestamp()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TextOutputTest>(configure=>configure.TimeStamp= XunitLoggerTimeStamp.DateTime);

        Assert.NotNull(log);

        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreateLoggerWithOffset()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TextOutputTest>(configure => configure.TimeStamp = XunitLoggerTimeStamp.Offset);

        Assert.NotNull(log);

        log.LogInformation("Here is some information");
    }

    [Fact]
    public void CreatedLoggerFiltersMessages()
    {
        WriteFunctionName();

        var wrapper = new WrappedOutput(output);

        var log = wrapper.CreateLogger<TextOutputTest>(configure => configure.MinLevel = LogLevel.Information);

        Assert.NotNull(log);

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = wrapper.Messages;

        Assert.Collection(messages, s => s.EndsWith(info));
    }
}