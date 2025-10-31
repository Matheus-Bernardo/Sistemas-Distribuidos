using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

var mongoConn = builder.Configuration.GetConnectionString("Mongo");
var mongoDbName = builder.Configuration.GetValue<string>("MongoDatabase") ?? "projetoc216";

var client = new MongoClient(mongoConn);
var database = client.GetDatabase(mongoDbName);

builder.Services.AddSingleton(database);
builder.Services.AddSingleton<IMongoRepository<User>>(sp => new MongoRepository<User>(database, "users"));
builder.Services.AddSingleton<IMongoRepository<Product>>(sp => new MongoRepository<Product>(database, "products"));
builder.Services.AddSingleton<IMongoRepository<Order>>(sp => new MongoRepository<Order>(database, "orders"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.MapControllers();
app.Run();