using System.Collections.Immutable;
using Xunit.Abstractions;

namespace C3D.Extensions.Logging.Xunit.Utilities;

public class LoggingMessageSink : IMessageSink
{
    private readonly List<IMessageSinkMessage> messages = new();
    private readonly IMessageSink output;

    public LoggingMessageSink(IMessageSink output) => this.output = output;

#if NET6_0_OR_GREATER
    public IReadOnlyList<IMessageSinkMessage> Messages => messages.ToImmutableList();
#else
    public IReadOnlyList<IMessageSinkMessage> Messages => ImmutableList<IMessageSinkMessage>.Empty.AddRange(messages);
#endif

    public bool OnMessage(IMessageSinkMessage message)
    {
        messages.Add(message);
        return output.OnMessage(message);
    }

    public void Clear() => messages.Clear();
}

