using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GerenciadorDeProdutos.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string UserId { get; set; } = null!;
    public List<OrderItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class OrderItem
{
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}