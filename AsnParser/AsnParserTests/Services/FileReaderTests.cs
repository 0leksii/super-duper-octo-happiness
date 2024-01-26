using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AsnParser.Services;
using AsnParser.Services.Interfaces;
using FluentAssertions;
using Xunit;

namespace AsnParserTests.Services;

public class FileReaderTests
{
    private readonly IFileReader _fileReader;

    public FileReaderTests()
    {
        _fileReader = new FileReader();
    }

    [Fact]
    public async Task ParseSimpleBox_ReturnsExpectedResults()
    {
        const string fileContent =
            "HDR  TRSP117                                           6874454I                           \nLINE P000001661         9781465121550         12     \nLINE P000001661         9925151267712         2      \nLINE P000001661         9651216865465         1   ";
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, fileContent);

        var result = await _fileReader.ParseFileAsync(path).ToListAsync();

        result.Should().ContainSingle();
    }
}