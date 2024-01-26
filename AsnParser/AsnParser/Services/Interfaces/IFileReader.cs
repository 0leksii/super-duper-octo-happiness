using AsnParser.Models;

namespace AsnParser.Services.Interfaces;

public interface IFileReader
{
    IAsyncEnumerable<Box> ParseFileAsync(string filePath);
}