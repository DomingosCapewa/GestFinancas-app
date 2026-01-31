using System;

namespace GestFinancas_Api.Models
{
  public class ConsentLog
  {
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string ConsentType { get; set; } = string.Empty; 
    public string Version { get; set; } = string.Empty;
    public bool Accepted { get; set; }
    public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
  }
}
