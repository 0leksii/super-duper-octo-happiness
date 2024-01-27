using System.Threading;
using System.Threading.Tasks;
using AsnParser.Services;
using AsnParser.Services.Interfaces;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace AsnParserTests.Services;

public class FileQueueTests
{
    private readonly IFileQueue _fileQueue;
    private readonly IFixture _fixture;

    public FileQueueTests()
    {
        _fixture = new Fixture();
        _fileQueue = new FileQueue();
    }

    [Fact]
    public async Task QueueMethods_ReturnExpectedResults()
    {
        var testString = _fixture.Create<string>();

        await _fileQueue.EnqueueFileParsingTaskAsync(testString);

        var result = await _fileQueue.DequeueFileParsingTaskAsync(CancellationToken.None);

        result.Should().Be(testString);
    }
}