using System.Threading.Channels;
using AsnParser.Services.Interfaces;

namespace AsnParser.Services;

public class FileQueue : IFileQueue
{
    private readonly Channel<string> _channel;
    public FileQueue()
    {
        var options = new BoundedChannelOptions(Environment.ProcessorCount)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<string>(options);
    }
    
    public ValueTask EnqueueFileParsingTaskAsync(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        
        return _channel.Writer.WriteAsync(filePath);
    }

    public ValueTask<string> DequeueFileParsingTaskAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}