using AsnParser.MapperProfiles;
using AsnParser.Repositories;
using AsnParser.Repositories.Interfaces;
using AsnParser.Services;
using AsnParser.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IFileQueue, FileQueue>();
builder.Services.AddSingleton<IFileReader, FileReader>();
builder.Services.AddHostedService<FileWatcher>();
builder.Services.AddHostedService<FileProcessor>();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(builder.Configuration["MongoDbUri"]));
builder.Services.AddSingleton<IBoxRepository, BoxRepository>();
builder.Services.AddAutoMapper(typeof(BoxProfile));

var host = builder.Build();

host.Run();