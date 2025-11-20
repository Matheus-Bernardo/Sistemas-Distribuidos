using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Repositories;

namespace GerenciadorDeProdutos.Services;

public class AuditService
{
    private readonly IMongoRepository<AuditLog> _repo;

    public AuditService(IMongoRepository<AuditLog> repo)
    {
        _repo = repo;
    }

    public async Task LogAsync(AuditLog log)
    {
        await _repo.CreateAsync(log);
    }
}