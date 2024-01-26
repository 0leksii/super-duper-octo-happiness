using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AsnParser.Models.Entities;

public record BoxEntity
{
    [BsonId] public ObjectId Id { get; set; }

    public string SupplierIdentifier { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;

    public IReadOnlyCollection<ContentEntity> Contents { get; init; } = new List<ContentEntity>();
}