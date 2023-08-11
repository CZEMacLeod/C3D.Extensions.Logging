using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging;

public static class MELXunitLoggerExtensions
{
    private static bool TryGetService<T>(this IServiceProvider services, [NotNullWhen(returnValue: true)] out T? service) where T : class
    {
        service = services.GetService<T>();
        return service is not null;
    }

    #region "ILoggingBuilder"
    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder)
    {
        LoggerProviderOptions.RegisterProviderOptions<XunitLoggerOptions, XunitLoggerProvider>(builder.Services);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XunitLoggerProvider>());
        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, Action<XunitLoggerOptions>? configure)
    {
        var optionsBuilder = builder.Services.AddOptions<XunitLoggerOptions>();
        if (configure is not null) { optionsBuilder.Configure(configure); }
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider>(sp =>
            {
                if (sp.TryGetService<ITestOutputHelper>(out var testOutput)) { ActivatorUtilities.CreateInstance<XunitLoggerProvider>(sp,testOutput); }
                if (sp.TryGetService<IMessageSink>(out var messageSink)) { ActivatorUtilities.CreateInstance<XunitLoggerProvider>(sp, messageSink); }
                return Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider.Instance;
            }));

        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null)
    {
        var optionsBuilder = builder.Services.AddOptions<XunitLoggerOptions>();
        if (configure is not null) { optionsBuilder.Configure(configure); }
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(sp=> ActivatorUtilities.CreateInstance<XunitLoggerProvider>(sp, output)));
        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, IMessageSink output, Action<XunitLoggerOptions>? configure = null)
    {
        var optionsBuilder = builder.Services.AddOptions<XunitLoggerOptions>();
        if (configure is not null) { optionsBuilder.Configure(configure); }
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider>(sp => ActivatorUtilities.CreateInstance<XunitLoggerProvider>(sp, output)));
        return builder;
    }
    #endregion

    #region "ILoggerFactory"
    public static ILoggerFactory AddXunit(this ILoggerFactory loggerFactory, ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        loggerFactory.AddProvider(new XunitLoggerProvider(output, options));
        return loggerFactory;
    }

    public static ILoggerFactory AddXunit(this ILoggerFactory loggerFactory, IMessageSink output, Action<XunitLoggerOptions>? configure = null)
    {
        var options = XunitLoggerOptions.CreateOptions(configure);
        loggerFactory.AddProvider(new XunitLoggerProvider(output, options));
        return loggerFactory;
    }
    #endregion
}
