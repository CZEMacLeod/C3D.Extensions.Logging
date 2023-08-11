using System.Collections.Immutable;
using Xunit.Abstractions;

namespace C3D.Extensions.Logging.Xunit.Utilities;

public class LoggingTestOutputHelper : ITestOutputHelper
{
    private readonly ITestOutputHelper output;
    private readonly List<string> messages = new();

#if NET6_0_OR_GREATER
    public IReadOnlyList<string> Messages => messages.ToImmutableList();
#else
    public IReadOnlyList<string> Messages => ImmutableList<string>.Empty.AddRange(messages);
#endif

    public void ClearMessages() { messages.Clear(); }

    public LoggingTestOutputHelper(ITestOutputHelper output) => this.output = output;

    public void WriteLine(string message)
    {
        output.WriteLine(message);
        messages.Add(message);
    }

    public void WriteLine(string format, params object[] args)
    {
        output.WriteLine(format, args);
        messages.Add(string.Format(format, args));
    }
}
