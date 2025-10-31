using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GerenciadorDeProdutos.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}