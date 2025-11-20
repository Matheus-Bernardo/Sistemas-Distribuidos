using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GerenciadorDeProdutos.Models;

public class AuditLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? UserId { get; set; }
    public string Endpoint { get; set; } = "";
    public string Method { get; set; } = "";
    public string Action { get; set; } = "";
    public string? Body { get; set; }
    public DateTime Timestamp { get; set; }
}