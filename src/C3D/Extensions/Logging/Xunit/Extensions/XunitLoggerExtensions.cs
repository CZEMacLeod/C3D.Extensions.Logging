using C3D.Extensions.Xunit.Logging;
using C3D.Extensions.Xunit.Logging.Loggers;
using Microsoft.Extensions.Logging;

namespace Xunit.Abstractions;

public static class XAXunitLoggerExtensions
{
    public static ILogger<T> CreateLogger<T>(this ITestOutputHelper output, LogLevel minLevel, DateTimeOffset? logStart = null) =>
        output.CreateLogger<T>(XunitLoggerOptions.CreateOptions(minLevel, logStart));

    public static ILogger<T> CreateLogger<T>(this IMessageSink output, LogLevel minLevel, DateTimeOffset? logStart = null) =>
        output.CreateLogger<T>(XunitLoggerOptions.CreateOptions(minLevel, logStart));

    public static ILogger<T> CreateLogger<T>(this ITestOutputHelper output, Action<XunitLoggerOptions>? configure = null) =>
        output.CreateLogger<T>(XunitLoggerOptions.CreateOptions(configure));

    public static ILogger<T> CreateLogger<T>(this IMessageSink output, Action<XunitLoggerOptions>? configure = null) =>
        output.CreateLogger<T>(XunitLoggerOptions.CreateOptions(configure));

    public static ILogger<T> CreateLogger<T>(this ITestOutputHelper output, XunitLoggerOptions options)
        => new TextOutputLogger<T>(output, () => options);

    public static ILogger<T> CreateLogger<T>(this IMessageSink output, XunitLoggerOptions options)
        => new MessageSinkLogger<T>(output, () => options);
}
