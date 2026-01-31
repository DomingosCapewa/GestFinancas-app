using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GestFinancas_Api.Models;
using System.Security.Claims;

namespace GestFinancas_Api.Middleware
{
  public class AuditMiddleware
  {
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
      var path = context.Request.Path.Value ?? string.Empty;
      var method = context.Request.Method;

      // Ignorar rotas não relevantes
      if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
      {
        await _next(context);
        return;
      }

      await _next(context);

      // Registrar apenas ações de escrita e endpoints sensíveis
      var shouldLog = method == HttpMethods.Post || method == HttpMethods.Put || method == HttpMethods.Delete;
      if (!shouldLog && !path.Contains("/login", StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      int? userId = null;
      var userIdString = context.User?.FindFirst("id")?.Value;
      if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out var parsedId))
      {
        userId = parsedId;
      }

      var audit = new AuditLog
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        Action = $"{method} {path}",
        Entity = path,
        EntityId = null,
        Path = path,
        Method = method,
        StatusCode = context.Response.StatusCode,
        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
        UserAgent = context.Request.Headers["User-Agent"].ToString(),
        Timestamp = DateTime.UtcNow
      };

      db.AuditLogs.Add(audit);
      await db.SaveChangesAsync();
    }
  }
}
