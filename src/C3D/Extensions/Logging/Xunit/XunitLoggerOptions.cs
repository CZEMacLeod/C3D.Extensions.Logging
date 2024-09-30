using Microsoft.Extensions.Logging;
using System.Globalization;

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

    public XunitLoggerOptions(Func<DateTimeOffset> getUtcNow) => 
        GetUtcNow = getUtcNow;

    public XunitLoggerOptions(TimeProvider timeProvider) : this(timeProvider.GetUtcNow) { }

    #region "TimeStamp"
    private Func<string?>? getTimeStamp;
    private int? prefixLength;
    private string? timeStampFormat;
    private XunitLoggerTimeStamp? timeStamp;
    private DateTimeOffset? logStart;

    public System.Globalization.CultureInfo? Culture { get; set; }

    public XunitLoggerTimeStamp TimeStamp
    {
        get => timeStamp ??= XunitLoggerTimeStamp.None;
        set
        {
            timeStamp = value;
            if (timeStamp == XunitLoggerTimeStamp.Offset && logStart is null)
            {
                logStart = GetUtcNow();
            }
        }
    }

    public string TimeStampFormat { 
        get => timeStampFormat ?? TimeStamp switch
        {
            XunitLoggerTimeStamp.DateTime => "u",
            XunitLoggerTimeStamp.Offset => "G",
            _ => ""
        };
        set => timeStampFormat = value;
    }

    public DateTimeOffset LogStart
    {
        get => logStart ??= GetUtcNow();
        set
        {
            logStart = value;
            timeStamp ??= XunitLoggerTimeStamp.Offset;
        }
    }

    public Func<string?> GetTimeStamp { 
        get => getTimeStamp ?? DefaultTimeStamp; 
        set => getTimeStamp = value; 
    }

    public Func<DateTimeOffset> GetUtcNow { get; set; }

    private string? DefaultTimeStamp() => TimeStamp switch
    {
        XunitLoggerTimeStamp.DateTime => GetUtcNow().ToString(TimeStampFormat, Culture ?? CultureInfo.CurrentCulture),
        XunitLoggerTimeStamp.Offset => (GetUtcNow() - LogStart).ToString(TimeStampFormat, Culture ?? CultureInfo.CurrentCulture),
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

    [Obsolete("Please use the overload for configuring")]
    internal static XunitLoggerOptions CreateOptions(LogLevel? minLevel = null, DateTimeOffset? logStart = null) =>
        CreateOptions(options =>
        {
            if (minLevel is not null) { options.MinLevel = minLevel.Value; }
            if (logStart is not null) { 
                options.LogStart = logStart.Value; 
                options.TimeStamp = XunitLoggerTimeStamp.Offset;
            }
        });

    internal static XunitLoggerOptions CreateOptions(Action<XunitLoggerOptions>? configure)
    {
        var options = new XunitLoggerOptions();
        configure?.Invoke(options);
        return options;
    }
}
