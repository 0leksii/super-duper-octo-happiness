using AsnParser.Models;
using AsnParser.Models.Entities;
using AsnParser.Repositories.Interfaces;
using AutoMapper;
using MongoDB.Driver;

namespace AsnParser.Repositories;

public class BoxRepository : IBoxRepository
{
    private const string DatabaseName = "AsnParser";
    private const string CollectionName = "Boxes";
    private readonly IMapper _mapper;
    private readonly IMongoClient _mongoClient;

    public BoxRepository(IMongoClient mongoClient, IMapper mapper)
    {
        _mongoClient = mongoClient;
        _mapper = mapper;
    }

    public Task SaveBoxAsync(Box box)
    {
        var collection = _mongoClient.GetDatabase(DatabaseName).GetCollection<BoxEntity>(CollectionName);

        return collection.InsertOneAsync(_mapper.Map<BoxEntity>(box));
    }
}