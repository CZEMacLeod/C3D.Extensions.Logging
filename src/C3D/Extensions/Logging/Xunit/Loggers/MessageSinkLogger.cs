using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace C3D.Core.Xunit.Logging.Loggers;

public class MessageSinkLogger : XunitLoggerBase
{
    private readonly IMessageSink output;
    
    public MessageSinkLogger(string category, IMessageSink output, Func<XunitLoggerOptions> options) :
        base(category, options) => this.output = output;

    public override void Log<TState>(
        LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = new DiagnosticMessage(category + ":" + formatter(state, exception));
        output.OnMessage(message);
    }
}

public class MessageSinkLogger<T> : MessageSinkLogger, ILogger<T>
{
    public MessageSinkLogger(IMessageSink output, Func<XunitLoggerOptions> options) :
        base(typeof(T).FullName!, output, options)
    { }
}