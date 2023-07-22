using C3D.Core.Xunit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    [Fact]
    public void CreateLogger()
    {
        WriteFunctionName();

        var log = output.CreateLogger<TextOutputTest>();
        
        Assert.NotNull(log);
        
        log.LogInformation("Here is some information");
    }
}