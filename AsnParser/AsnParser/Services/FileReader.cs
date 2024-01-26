using System.Text.RegularExpressions;
using AsnParser.Models;
using AsnParser.Services.Interfaces;

namespace AsnParser.Services;

public class FileReader : IFileReader
{
    private static readonly Regex BoxRegexp = new(@"HDR\s+(?<suppliedIdentifier>\w+)\s+(?<boxIdentifier>\w+)\s*");
    private static readonly Regex ContentRegexp = new(@"LINE\s+(?<poNumber>\w+)\s+(?<isbn>\w+)\s+(?<quantity>\d+)\s*");
    public async IAsyncEnumerable<Box> ParseFileAsync(string filePath)
    {
        Box? currentBox = null;
        var contents = new List<Content>();
        using var reader = File.OpenText(filePath);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (TryParseContent(line, out var content))
            {
                contents.Add(content!);
            }
            
            if (TryParseBox(line, out var newBox))
            {
                if (currentBox != null)
                {
                    var boxToReturn = currentBox with { Contents = contents };
                    currentBox = newBox!;
                    contents = new List<Content>();
                    yield return boxToReturn;
                }
                else
                {
                    currentBox = newBox!;
                }
            }
            
        }
        if (currentBox != null)
        {
            yield return currentBox with { Contents = contents };
        }
    }

    private static bool TryParseBox(string fileLine, out Box? box)
    {
        var boxMatch = BoxRegexp.Match(fileLine);
        if (boxMatch.Success)
        {
            box = new Box
            {
                Identifier = boxMatch.Groups["boxIdentifier"].Value,
                SupplierIdentifier = boxMatch.Groups["suppliedIdentifier"].Value
            };
            return true;
        }

        box = null;
        return false;
    }
    
    private static bool TryParseContent(string fileLine, out Content? content)
    {
        var contentMatch = ContentRegexp.Match(fileLine);
        if (contentMatch.Success)
        {
            content = new Content
            {
                Isbn = contentMatch.Groups["isbn"].Value,
                Quantity = Convert.ToInt32(contentMatch.Groups["quantity"].Value),
                PoNumber = contentMatch.Groups["poNumber"].Value
            };
            return true;
        }

        content = null;
        return false;
    }
}