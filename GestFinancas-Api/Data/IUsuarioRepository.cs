using GestFinancas_Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestFinancas_Api.Data
{
  public interface IUsuarioRepository
  {
    Task<List<Usuario>> ObterTodosUsuariosAsync();
    Task<Usuario?> ObterUsuarioPorIdAsync(int id);
    Task<Usuario?> ObterUsuarioPorEmailSenhaAsync(string email, string senha);
    Task<int> AddUsuarioAsync(Usuario usuario);
    Task<Usuario?> ObterUsuarioPorEmailAsync(string email);
    Task<int> AtualizarUsuarioAsync(Usuario usuario);
    Task<Usuario?> RecuperarSenha(string email);
    Task<Usuario?> BuscarUsuarioPorEmail(string email);
    Task<Usuario> BuscarUsuarioPorToken(string token);
    Task<bool> ExisteAdminAsync();
    Task<bool> PromoverUsuarioAdminAsync(string email);

    // Task<Usuario?> ResetarSenhaUsuario(string email, string novaSenha);
    // Task<int> DeleteUsuarioAsync(int id);
  }
}
