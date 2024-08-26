using System.Globalization;

namespace C3D.Extensions.Xunit.Logging;

public static class XunitLoggerOptionsExtensions { 
    public static XunitLoggerOptions WithTimeStamp(this XunitLoggerOptions options, XunitLoggerTimeStamp timeStamp)
    {
        options.TimeStamp = timeStamp;
        return options;
    }

    public static XunitLoggerOptions WithTimeStampFormat(this XunitLoggerOptions options, string timeStampFormat)
    {
        options.TimeStampFormat = timeStampFormat;
        return options;
    }

    public static XunitLoggerOptions UseTimeProvider(this XunitLoggerOptions options, TimeProvider timeProvider)
    {
        options.GetUtcNow = timeProvider.GetUtcNow;
        return options;
    }

    public static XunitLoggerOptions Restart(this XunitLoggerOptions options)
    {
        options.LogStart = options.GetUtcNow();
        return options;
    }

    public static XunitLoggerOptions UseCulture(this XunitLoggerOptions options, CultureInfo culture)
    {
        options.Culture = culture;
        return options;
    }
}
