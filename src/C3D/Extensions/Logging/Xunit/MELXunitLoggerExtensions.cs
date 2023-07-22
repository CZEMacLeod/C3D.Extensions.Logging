using C3D.Core.Xunit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging;

public static class MELXunitLoggerExtensions
{
    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder)
    {
        LoggerProviderOptions.RegisterProviderOptions<XunitLoggerOptions, XunitLoggerProvider>(builder.Services);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XunitLoggerProvider>());
        return builder;
    }

    private static bool TryGetService<T>(this IServiceProvider services, [NotNullWhen(returnValue: true)] out T? service) where T : class
    {
        service = services.GetService<T>();
        return service is not null;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, Action<XunitLoggerOptions>? configure)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider>(sp =>
            {
                if (sp.TryGetService<ITestOutputHelper>(out var testOutput)) { return new XunitLoggerProvider(testOutput, options); }
                if (sp.TryGetService<IMessageSink>(out var messageSink)) { return new XunitLoggerProvider(messageSink, options); }
                return Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider.Instance;
            }));

        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(new XunitLoggerProvider(output, options)));
        return builder;
    }

    public static ILoggerFactory AddXunit(this ILoggerFactory loggerFactory, ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        loggerFactory.AddProvider(new XunitLoggerProvider(output, options));
        return loggerFactory;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, IMessageSink output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(new XunitLoggerProvider(output, options)));
        return builder;
    }

    public static ILoggerFactory AddXunit(this ILoggerFactory loggerFactory, IMessageSink output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        loggerFactory.AddProvider(new XunitLoggerProvider(output, options));
        return loggerFactory;
    }
}
