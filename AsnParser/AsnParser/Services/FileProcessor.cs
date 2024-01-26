using AsnParser.Repositories.Interfaces;
using AsnParser.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AsnParser.Services;

public class FileProcessor : BackgroundService
{
    private readonly IBoxRepository _boxRepository;
    private readonly IFileQueue _fileQueue;
    private readonly IFileReader _fileReader;
    private readonly ILogger<FileProcessor> _logger;

    public FileProcessor(IFileReader fileReader, IFileQueue fileQueue, IBoxRepository boxRepository,
        ILogger<FileProcessor> logger)
    {
        _fileReader = fileReader;
        _fileQueue = fileQueue;
        _boxRepository = boxRepository;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting file processor");
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var filePath = await _fileQueue.DequeueFileParsingTaskAsync(stoppingToken);
                await foreach (var box in _fileReader.ParseFileAsync(filePath).WithCancellation(stoppingToken))
                    await _boxRepository.SaveBoxAsync(box);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while processing the queue");
            }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File processor is stopping");
        await base.StopAsync(stoppingToken);
    }
}