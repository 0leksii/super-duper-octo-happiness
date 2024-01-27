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
    private FileSystemWatcher? _fileSystemWatcher;

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
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _fileSystemWatcher = new FileSystemWatcher(path);
        _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes
                                          | NotifyFilters.CreationTime
                                          | NotifyFilters.DirectoryName
                                          | NotifyFilters.FileName
                                          | NotifyFilters.LastAccess
                                          | NotifyFilters.LastWrite
                                          | NotifyFilters.Security
                                          | NotifyFilters.Size;
        _fileSystemWatcher.Created += async (sender, args) =>
        {
            _logger.LogInformation("Adding {File} to the queue", args.FullPath);
            await _fileQueue.EnqueueFileParsingTaskAsync(args.FullPath);
        };
        _fileSystemWatcher.IncludeSubdirectories = true;
        _fileSystemWatcher.EnableRaisingEvents = true;

        while (!stoppingToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        _logger.LogInformation("Exiting the loop");
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File watcher is stopping");
        await base.StopAsync(stoppingToken);
    }
}