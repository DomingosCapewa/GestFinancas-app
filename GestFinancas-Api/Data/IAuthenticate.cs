using System.Threading.Tasks;

namespace GestFinancas_Api.Data
{
  public interface IAuthenticate
  {
    Task<bool> AuthenticateAscync(string email, string senha);
    Task<bool> usuarioExiste(string email);

    public string GenerateToken(int id, string email);
  }
}