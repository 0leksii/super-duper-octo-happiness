namespace AsnParser.Models;

public record Content
{
    public string PoNumber { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public int Quantity { get; init; }
}