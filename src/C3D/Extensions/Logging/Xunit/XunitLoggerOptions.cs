using Microsoft.Extensions.Logging;

namespace C3D.Core.Xunit.Logging;

public enum XunitLoggerTimeStamp
{
    None,
    Offset,
    DateTime
}

public class XunitLoggerOptions
{
    #region "TimeStamp"
    private Func<string>? getTimeStamp;

    public XunitLoggerTimeStamp TimeStamp { get; set; } = XunitLoggerTimeStamp.None;
    public string TimeStampFormat { get; set; } = "G";

    public DateTimeOffset LogStart { get; set; } = DateTimeOffset.UtcNow;

    public Func<string> GetTimeStamp { get => getTimeStamp ?? DefaultTimeStamp; set => getTimeStamp = value; }

    private string DefaultTimeStamp()
    {
        return TimeStamp switch
        {
            XunitLoggerTimeStamp.DateTime => DateTimeOffset.UtcNow.ToString(TimeStampFormat),
            XunitLoggerTimeStamp.Offset => (DateTimeOffset.UtcNow - LogStart).ToString(TimeStampFormat),
            _ => string.Empty
        };
    }
    #endregion

    public int PrefixLength { get; set; } = 30;

    public LogLevel MinLevel { get; set; } = LogLevel.Trace;

    internal static XunitLoggerOptions CreateOptions(LogLevel? minLevel = null, DateTimeOffset? logStart = null) =>
        CreateOptions(options =>
        {
            if (minLevel is not null) { options.MinLevel = minLevel.Value; }
            if (logStart is not null) { options.LogStart = logStart.Value; }
        });

    internal static XunitLoggerOptions CreateOptions(Action<XunitLoggerOptions>? configure)
    {
        var options = new XunitLoggerOptions();
        configure?.Invoke(options);
        return options;
    }
}
