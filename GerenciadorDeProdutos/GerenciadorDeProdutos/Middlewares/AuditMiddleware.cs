using System.Text;
using GerenciadorDeProdutos.Models;
using GerenciadorDeProdutos.Services;

namespace GerenciadorDeProdutos.Middlewares;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuditService auditService)
    {
        // Lê o body caso exista
        context.Request.EnableBuffering();
        string body = "";
        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // reset stream
        }

        // Recuperar usuário do token (se existir)
        var userId = context.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value ?? "anonymous";

        // Salvar log
        var log = new AuditLog
        {
            UserId = userId,
            Endpoint = context.Request.Path,
            Method = context.Request.Method,
            Action = $"Chamada ao endpoint {context.Request.Path}",
            Body = body,
            Timestamp = DateTime.UtcNow
        };

        await auditService.LogAsync(log);

        await _next(context);
    }
}