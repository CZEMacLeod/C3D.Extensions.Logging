using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace C3D.Extensions.Xunit.Logging.Loggers;

public class TextOutputLogger : XunitLoggerBase
{
    private readonly ITestOutputHelper _output;

    public TextOutputLogger(string category, ITestOutputHelper output, Func<XunitLoggerOptions> options) :
        base(category, options) => _output = output;

    public override void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        string message = BuildMessageText(logLevel, state, exception, formatter);

        try
        {
            _output.WriteLine(message);
        }
        catch (Exception)
        {
        }
    }
}

public class TextOutputLogger<T> : TextOutputLogger, ILogger<T>
{
    public TextOutputLogger(ITestOutputHelper output, Func<XunitLoggerOptions> options) :
        base(typeof(T).FullName!, output, options)
    { }
}
