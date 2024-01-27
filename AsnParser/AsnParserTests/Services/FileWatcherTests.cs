using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AsnParser.Services;
using AsnParser.Services.Interfaces;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AsnParserTests.Services;

public class FileWatcherTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IFileQueue> _fileQueueMock;
    private readonly FileWatcher _fileWatcher;
    private readonly IFixture _fixture;
    private readonly LoggerSpy<FileWatcher> _loggerSpy;

    public FileWatcherTests()
    {
        _loggerSpy = new LoggerSpy<FileWatcher>();
        _fileQueueMock = new Mock<IFileQueue>(MockBehavior.Strict);
        _configurationMock = new Mock<IConfiguration>(MockBehavior.Strict);
        _fixture = new Fixture();
        _fileWatcher = new FileWatcher(_fileQueueMock.Object, _configurationMock.Object, _loggerSpy);
    }

    [Fact]
    public async Task StopAsync_ThrowsAnyException()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;
        source.Cancel();

        await _fileWatcher.StopAsync(token);

        _loggerSpy.State.Should().HaveCount(1);
    }

    [Fact]
    public async Task StartAsync_WatchesFiles()
    {
        var tempFolder = Path.GetTempPath();
        var folderName = _fixture.Create<string>();
        var fileName = _fixture.Create<string>();
        var source = new CancellationTokenSource();
        var token = source.Token;
        _configurationMock.SetupGet(_ => _["Directory"]).Returns(Path.Combine(tempFolder, folderName)).Verifiable();
        _fileQueueMock.Setup(_ => _.EnqueueFileParsingTaskAsync(Path.Combine(tempFolder, folderName, fileName)))
            .Returns(ValueTask.CompletedTask).Verifiable();
        Directory.CreateDirectory(Path.Combine(tempFolder, folderName));

        var task = _fileWatcher.StartAsync(token);

        await File.Create(Path.Combine(tempFolder, folderName, fileName)).DisposeAsync();

        await Task.Delay(TimeSpan.FromSeconds(3));
        source.Cancel();
        await task;

        _configurationMock.Verify();
        _fileQueueMock.Verify();
    }
}