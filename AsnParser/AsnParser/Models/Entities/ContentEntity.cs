namespace AsnParser.Models.Entities;

public record ContentEntity
{
    public string PoNumber { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public int Quantity { get; set; } = 0;
}