using C3D.Core.Xunit.Logging.Loggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace C3D.Core.Xunit.Logging;

[ProviderAlias("Xunit")]
public sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper? output1;
    private readonly IMessageSink? output2;
    private readonly IDisposable? onChange;
    private XunitLoggerOptions options;
    private bool disposedValue;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

    public XunitLoggerProvider(IMessageSink output, IOptionsMonitor<XunitLoggerOptions> options)
    {
        output2= output;
        onChange = options.OnChange(SetOptions);
        SetOptions(options.CurrentValue);
    }

    public XunitLoggerProvider(ITestOutputHelper output, IOptionsMonitor<XunitLoggerOptions> options)
    {
        output1 = output;
        onChange = options.OnChange(SetOptions);
        SetOptions(options.CurrentValue);
    }

    [MemberNotNull(nameof(this.options))]
    private void SetOptions(XunitLoggerOptions options) => this.options = options;

    internal XunitLoggerProvider(IMessageSink output, XunitLoggerOptions options)
    {
        output2 = output;
        SetOptions(options);
    }

    internal XunitLoggerProvider(ITestOutputHelper output, XunitLoggerOptions options)
    {
        output1 = output;
        SetOptions(options);
    }

    internal XunitLoggerOptions GetOptions() => options;

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, 
            name => output1 is null ? 
                    (output2 is null ? NullLogger.Instance : 
                     new MessageSinkLogger(name, output2, GetOptions)) :
                     new TextOutputLogger(name, output1, GetOptions));

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                onChange?.Dispose();
            }
            _loggers.Clear();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
