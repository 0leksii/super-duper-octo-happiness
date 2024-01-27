using System.Threading.Tasks;
using AsnParser.MapperProfiles;
using AsnParser.Models;
using AsnParser.Models.Entities;
using AsnParser.Repositories;
using AsnParser.Repositories.Interfaces;
using AutoFixture;
using AutoMapper;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AsnParserTests.Repositories;

public class BoxRepositoryTests
{
    private readonly IBoxRepository _boxRepository;
    private readonly IFixture _fixture;
    private readonly Mock<IMongoClient> _mongoClientMock;

    public BoxRepositoryTests()
    {
        _fixture = new Fixture();
        _mongoClientMock = new Mock<IMongoClient>(MockBehavior.Strict);
        var mapperConfiguration = new MapperConfiguration(cfg => { cfg.AddProfile<BoxProfile>(); });
        _boxRepository = new BoxRepository(_mongoClientMock.Object, mapperConfiguration.CreateMapper());
    }

    [Fact]
    public async Task SaveBoxAsync_ExecutesSuccessfully()
    {
        var box = _fixture.Create<Box>();
        var mongoDbMock = new Mock<IMongoDatabase>(MockBehavior.Strict);
        var mongoCollectionMock = new Mock<IMongoCollection<BoxEntity>>(MockBehavior.Strict);
        mongoDbMock.Setup(_ => _.GetCollection<BoxEntity>("Boxes", null)).Returns(mongoCollectionMock.Object)
            .Verifiable();
        mongoCollectionMock
            .Setup(_ => _.InsertOneAsync(
                It.Is<BoxEntity>(val =>
                    val.Identifier == box.Identifier && val.SupplierIdentifier == box.SupplierIdentifier &&
                    val.Contents.Count == box.Contents.Count), null, default)).Returns(Task.CompletedTask).Verifiable();
        _mongoClientMock.Setup(_ => _.GetDatabase("AsnParser", null)).Returns(mongoDbMock.Object).Verifiable();

        await _boxRepository.SaveBoxAsync(box);

        _mongoClientMock.Verify();
        mongoDbMock.Verify();
        mongoCollectionMock.Verify();
    }
}