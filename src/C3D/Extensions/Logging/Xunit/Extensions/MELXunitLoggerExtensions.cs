using C3D.Extensions.Xunit.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Xunit.Abstractions;

namespace Microsoft.Extensions.Logging;

public static class MELXunitLoggerExtensions
{
    #region "ILoggingBuilder"
    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, Action<XunitLoggerOptions>? configure = null)
    {
        LoggerProviderOptions.RegisterProviderOptions<XunitLoggerOptions, XunitLoggerProvider>(builder.Services);
        if (configure is not null) { builder.Services.Configure(configure); }
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, XunitLoggerProvider>());
        return builder;
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null)
    {
        builder.Services.AddSingleton(output);
        return builder.AddXunit(configure);
    }

    public static ILoggingBuilder AddXunit(this ILoggingBuilder builder, IMessageSink output, Action<XunitLoggerOptions>? configure = null)
    {
        builder.Services.AddSingleton(output);
        return builder.AddXunit(configure);
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
