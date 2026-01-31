using System.ComponentModel.DataAnnotations;

namespace GestFinancas_Api.Dtos
{
  public class ConsentDto
  {
    [Required]
    [StringLength(50)]
    public string ConsentType { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Version { get; set; } = string.Empty;

    public bool Accepted { get; set; } = true;
  }
}
