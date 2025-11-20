using System.Text;
using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Services;

namespace GerenciadorDeProdutos.Middlewares;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    // Rotas que NÃO devem ter o body registrado
    private static readonly List<string> SensitiveEndpoints = new()
    {
        "/api/auth/login",
        "/api/auth/register",
    };

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuditService auditService)
    {
        string body = "";

        // Habilita leitura do body
        context.Request.EnableBuffering();

        // Se o body existe e a rota NÃO é sensível → lê o body
        var path = context.Request.Path.Value!.ToLower();

        if (context.Request.ContentLength > 0 &&
            !SensitiveEndpoints.Contains(path))
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // Recupera usuário do token
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "anonymous";

        // Cria log
        var log = new AuditLog
        {
            UserId = userId,
            Endpoint = context.Request.Path,
            Method = context.Request.Method,
            Action = $"Chamada ao endpoint {context.Request.Path}",
            Body = string.IsNullOrWhiteSpace(body) ? null : body,
            Timestamp = DateTime.UtcNow
        };

        await auditService.LogAsync(log);

        await _next(context);
    }
}