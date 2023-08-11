using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace C3D.Extensions.Logging.Xunit.Test;

public class FixtureTests : IClassFixture<LoggingTestFixture>
{
    private readonly LoggingTestFixture fixture;
    private readonly ITestOutputHelper output;

    private void WriteFunctionName([CallerMemberName] string? caller = null) => output.WriteLine(caller);

    public FixtureTests(LoggingTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.output = output;
    }

    [Fact]
    public void CreateLoggerFromServices()
    {
        WriteFunctionName();

        fixture.ClearMessages();

        var services = fixture.Services;

        var log = services.GetRequiredService<ILogger<FixtureTests>>();

        Assert.NotNull(log);

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = fixture.Messages;

        Assert.NotNull(messages);

        Assert.Equal(2, messages.Count);

        var diagnosticMessages = messages.OfType<DiagnosticMessage>();

        Assert.Collection(diagnosticMessages, s => s.Message.EndsWith(debug), s => s.Message.EndsWith(info));
    }

    [Fact]
    public void CreateLoggerFromFactory()
    {
        WriteFunctionName();

        fixture.ClearMessages();

        var factory = fixture.LoggerFactory;

        var log = factory.CreateLogger<TestOutputHelperTests>();

        Assert.NotNull(log);

        const string debug = "Here is some debugging";
        const string info = "Here is some information";

        log.LogDebug(debug);
        log.LogInformation(info);

        var messages = fixture.Messages;

        var diagnosticMessages = messages.OfType<DiagnosticMessage>();

        Assert.Collection(diagnosticMessages, s => s.Message.EndsWith(debug), s => s.Message.EndsWith(info));
    }
}
