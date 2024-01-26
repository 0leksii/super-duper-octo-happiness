namespace AsnParser.Services.Interfaces;

public interface IFileQueue
{
    ValueTask EnqueueFileParsingTaskAsync(string filePath);
    ValueTask<string> DequeueFileParsingTaskAsync(CancellationToken cancellationToken);
}