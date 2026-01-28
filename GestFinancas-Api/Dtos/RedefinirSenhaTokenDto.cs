namespace GestFinancas_Api.Dtos
{
    public class RedefinirSenhaTokenDto
    {
        public string Token { get; set; }
        public string NovaSenha { get; set; } //verificar a lógica pois, estou salvandoa senha hash e não apenas "senha"
    }
}
