using C3D.Extensions.Xunit.Logging.Loggers;
using Microsoft.Extensions.Logging;
using System.Text;

namespace C3D.Extensions.Xunit.Logging.Loggers;

public abstract class XunitLoggerBase : ILogger
{
    protected readonly string category;
    private readonly Func<XunitLoggerOptions> options;

    public XunitLoggerBase(string category, Func<XunitLoggerOptions> options)
    {
        this.category = category;
        this.options = options;
    }

    public XunitLoggerOptions Options => options();

    private static readonly string[] NewLineChars = new[] { Environment.NewLine };

    protected string BuildMessageText<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var messageBuilder = new StringBuilder();
        var options = Options;

        var timeStamp = options.GetTimeStamp();

        var exceptionLinePrefix = exception?.GetType().FullName ?? string.Empty;
        var lines = formatter(state, exception).Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
        messageBuilder.Append("| ");
        if (timeStamp is not null)
        {
            messageBuilder.Append(timeStamp);
            messageBuilder.Append(" ");
        }
        messageBuilder.Append(category);
        messageBuilder.Append(" ");
        messageBuilder.Append(logLevel.ToString());
        var targetLength = Math.Max(options.PrefixLength - 3, exceptionLinePrefix.Length + 2);
        if (messageBuilder.Length< targetLength)
        {
            messageBuilder.Append(new string(' ', targetLength - messageBuilder.Length));
        }
        var firstLineLength = messageBuilder.Length - 2;
        messageBuilder.Append(" | ");
        messageBuilder.AppendLine(lines.FirstOrDefault() ?? string.Empty);

        var additionalLinePrefix = $"| {new string(' ', firstLineLength)} | ";
        foreach (var line in lines.Skip(1))
        {
            messageBuilder.Append(additionalLinePrefix);
            messageBuilder.AppendLine(line);
        }

        if (exception != null)
        {
            lines = exception.Message.Split(NewLineChars, StringSplitOptions.RemoveEmptyEntries);
            messageBuilder.Append($"| {exceptionLinePrefix.PadRight(firstLineLength)} | ");
            messageBuilder.AppendLine(lines.FirstOrDefault() ?? string.Empty);

            foreach (var line in lines.Skip(1))
            {
                messageBuilder.Append(additionalLinePrefix);
                messageBuilder.AppendLine(line);
            }
        }

        var message = messageBuilder.ToString();
        if (message.EndsWith(Environment.NewLine))
        {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            message = message[..^Environment.NewLine.Length];
#else
            message = message.Substring(0, message.Length - Environment.NewLine.Length);
#endif
        }

        return message;
    }

    #region "Scope"
#if NET7_0_OR_GREATER
public virtual IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
#else
    private class NullScope : IDisposable
    {   
        public void Dispose()
        {
        }
    }
    public virtual IDisposable BeginScope<TState>(TState state) => new NullScope();
#endif
#endregion

    public virtual bool IsEnabled(LogLevel logLevel) => logLevel >= options().MinLevel;
    public abstract void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter);

}
