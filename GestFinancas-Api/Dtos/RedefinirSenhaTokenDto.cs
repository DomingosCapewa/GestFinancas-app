using System.ComponentModel.DataAnnotations;

namespace GestFinancas_Api.Dtos
{
    public class RedefinirSenhaTokenDto
    {
        [Required]
        [StringLength(256, MinimumLength = 10)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NovaSenha { get; set; } = string.Empty; //verificar a lógica pois, estou salvandoa senha hash e não apenas "senha"
    }
}
