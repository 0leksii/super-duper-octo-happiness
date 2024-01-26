using AsnParser.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AsnParser.Services;

public class FileWatcher : BackgroundService
{
    private const string FilePath = "/tmp/spooler";
    private readonly IConfiguration _configuration;
    private readonly IFileQueue _fileQueue;
    private readonly ILogger<FileWatcher> _logger;

    public FileWatcher(IFileQueue fileQueue, IConfiguration configuration, ILogger<FileWatcher> logger)
    {
        _fileQueue = fileQueue;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var path = _configuration["Directory"] ?? FilePath;
        _logger.LogInformation("Starting the file watcher for {Path}", path);
        var fileSystemWatcher = new FileSystemWatcher(path);
        fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                         | NotifyFilters.CreationTime
                                         | NotifyFilters.DirectoryName
                                         | NotifyFilters.FileName
                                         | NotifyFilters.LastAccess
                                         | NotifyFilters.LastWrite
                                         | NotifyFilters.Security
                                         | NotifyFilters.Size;
        fileSystemWatcher.Created += async (sender, args) =>
        {
            _logger.LogInformation("Adding {File} to the queue", args.FullPath);
            await _fileQueue.EnqueueFileParsingTaskAsync(args.FullPath);
        };
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.EnableRaisingEvents = true;

        while (!stoppingToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File watcher is stopping");
        await base.StopAsync(stoppingToken);
    }
}