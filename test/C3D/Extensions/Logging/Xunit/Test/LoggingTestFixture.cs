using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using C3D.Extensions.Logging.Xunit.Utilities;

namespace C3D.Extensions.Logging.Xunit.Test;

public partial class LoggingTestFixture
{
    private readonly LoggingMessageSink output;
    public LoggingTestFixture(IMessageSink output) => this.output = new LoggingMessageSink(output);


    public IReadOnlyCollection<IMessageSinkMessage> Messages => output.Messages;
    public void ClearMessages() => output.Clear();

    private static IConfiguration CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        return builder.Build();

    }

    private ILoggingBuilder ConfigureLogger(ILoggingBuilder builder, IConfiguration configuration) => builder
            .ClearProviders()
            .SetMinimumLevel(LogLevel.Debug)
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddDebug();

    private IServiceCollection CreateServices()
    {
        var configuration = CreateConfiguration();

        return new ServiceCollection()
                        .AddSingleton(configuration)
                        .AddSingleton<IMessageSink>(output)
                        .AddTransient(_ => output.Messages)
                        .AddLogging(logging => ConfigureLogger(logging, configuration).AddXunit());
    }

    private IServiceProvider? services;
    public IServiceProvider Services => services ??= CreateServices().BuildServiceProvider();

    private ILoggerFactory CreateLoggerFactory()
    {
        var configuration = CreateConfiguration();
        return Microsoft.Extensions.Logging.LoggerFactory.Create(logging => ConfigureLogger(logging, configuration).AddXunit(output));
    }

    private ILoggerFactory? loggerFactory;
    public ILoggerFactory LoggerFactory => loggerFactory ??= CreateLoggerFactory();
}
