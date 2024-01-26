namespace AsnParser.Models;

public record Box
{
    public string SupplierIdentifier { get; init; } = string.Empty;
    public string Identifier { get; init; } = string.Empty;

    public IReadOnlyCollection<Content> Contents { get; init; } = new List<Content>();
}