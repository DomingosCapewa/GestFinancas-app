using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
namespace GestFinancas_Api.Models
{
  public class Usuario
  {
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; } = string.Empty;
    public string? Senha { get; set; } 
    public DateTime? DataCadastro { get; set; } = DateTime.UtcNow;
    public DateTime? TokenExpiracao { get; set; }

    [NotMapped]
    public string? Token { get; set; }

    public string? SenhaHash { get; set; } 
    public string? SenhaSalt { get; set; }

    public bool IsValid()
    {
      return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Senha);
    }
  }
}