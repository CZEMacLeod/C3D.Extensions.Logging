using Microsoft.Extensions.Logging;

namespace C3D.Extensions.Xunit.Logging;

public enum XunitLoggerTimeStamp
{
    None,
    Offset,
    DateTime
}

public class XunitLoggerOptions
{
    public XunitLoggerOptions() : this(() => DateTimeOffset.UtcNow) { }

    public XunitLoggerOptions(Func<DateTimeOffset> getUtcNow)
    {
        GetUtcNow = getUtcNow;
        LogStart = GetUtcNow();
    }

    public XunitLoggerOptions(TimeProvider timeProvider) : this(timeProvider.GetUtcNow) { }

    #region "TimeStamp"
    private Func<string?>? getTimeStamp;
    private int? prefixLength;
    private string? timeStampFormat;

    public XunitLoggerTimeStamp TimeStamp { get; set; } = XunitLoggerTimeStamp.None;
    public string TimeStampFormat { 
        get => timeStampFormat ?? TimeStamp switch
        {
            XunitLoggerTimeStamp.DateTime => "u",
            XunitLoggerTimeStamp.Offset => "G",
            _ => ""
        };
        set => timeStampFormat = value; }
    public DateTimeOffset LogStart { get; set; }

    public Func<string?> GetTimeStamp { get => getTimeStamp ?? DefaultTimeStamp; set => getTimeStamp = value; }

    public Func<DateTimeOffset> GetUtcNow { get; set; }

    private string? DefaultTimeStamp() => TimeStamp switch
    {
        XunitLoggerTimeStamp.DateTime => GetUtcNow().ToString(TimeStampFormat),
        XunitLoggerTimeStamp.Offset => (GetUtcNow() - LogStart).ToString(TimeStampFormat),
        _ => null
    };
    #endregion

    public int PrefixLength
    {
        get => prefixLength ?? TimeStamp switch
        {
            XunitLoggerTimeStamp.DateTime => 90,
            XunitLoggerTimeStamp.Offset => 85,
            _ => 70
        };
        set => prefixLength = value;
    }

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
