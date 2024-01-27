using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsnParser.Models;
using AsnParser.Repositories.Interfaces;
using AsnParser.Services;
using AsnParser.Services.Interfaces;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace AsnParserTests.Services;

public class FileProcessorTests
{
    private readonly Mock<IBoxRepository> _boxRepositoryMock;
    private readonly FileProcessor _fileProcessor;
    private readonly Mock<IFileQueue> _fileQueueMock;
    private readonly Mock<IFileReader> _fileReaderMock;
    private readonly IFixture _fixture;
    private readonly LoggerSpy<FileProcessor> _loggerSpy;

    public FileProcessorTests()
    {
        _fileQueueMock = new Mock<IFileQueue>(MockBehavior.Strict);
        _boxRepositoryMock = new Mock<IBoxRepository>(MockBehavior.Strict);
        _fileReaderMock = new Mock<IFileReader>(MockBehavior.Strict);
        _fixture = new Fixture();
        _loggerSpy = new LoggerSpy<FileProcessor>();

        _fileProcessor = new FileProcessor(_fileReaderMock.Object, _fileQueueMock.Object, _boxRepositoryMock.Object,
            _loggerSpy);
    }

    [Fact]
    public async Task ExecuteAsync_ExecutesSuccessfully()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;
        var filePath = _fixture.Create<string>();
        var box = _fixture.Create<Box>();
        _fileQueueMock.Setup(_ => _.DequeueFileParsingTaskAsync(It.IsAny<CancellationToken>())).ReturnsAsync(filePath)
            .Verifiable();
        _fileReaderMock.Setup(_ => _.ParseFileAsync(filePath)).Returns(new List<Box> { box }.ToAsyncEnumerable())
            .Verifiable();
        _boxRepositoryMock.Setup(_ => _.SaveBoxAsync(It.Is<Box>(val => val == box))).Returns(Task.CompletedTask)
            .Callback(() => source.Cancel()).Verifiable();

        await _fileProcessor.StartAsync(token);

        _fileQueueMock.Verify();
        _fileReaderMock.Verify();
        _boxRepositoryMock.Verify();
        _loggerSpy.State.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsOperationCanceledException()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;
        _fileQueueMock.Setup(_ => _.DequeueFileParsingTaskAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException()).Callback(() => source.Cancel()).Verifiable();

        await _fileProcessor.StartAsync(token);

        _fileQueueMock.Verify();
        _loggerSpy.State.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsAnyException()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;
        _fileQueueMock.Setup(_ => _.DequeueFileParsingTaskAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception()).Callback(() => source.Cancel()).Verifiable();

        await _fileProcessor.StartAsync(token);

        _fileQueueMock.Verify();
        _loggerSpy.State.Should().HaveCount(3);
    }

    [Fact]
    public async Task StopAsync_ThrowsAnyException()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;
        source.Cancel();

        await _fileProcessor.StopAsync(token);

        _loggerSpy.State.Should().HaveCount(1);
    }
}