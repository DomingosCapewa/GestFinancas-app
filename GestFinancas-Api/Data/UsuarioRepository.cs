using GestFinancas_Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestFinancas_Api.Data
{
  public class UsuarioRepository : IUsuarioRepository
  {
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<List<Usuario>> ObterTodosUsuariosAsync()
    {
      return await _context.Usuario.ToListAsync();
    }
public async Task<Usuario> BuscarUsuarioPorToken(string token)
    {
        return await _context.Usuario
        
        .FirstOrDefaultAsync(u => u.Token == token);
    }
    public async Task<Usuario?> ObterUsuarioPorIdAsync(int id)
    {
      return await _context.Usuario
          .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> ObterUsuarioPorEmailSenhaAsync(string email, string senha)

    {
      return await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);
    }

    public async Task<Usuario?> BuscarUsuarioPorEmail(string email)
    {
      return await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email == email);
    }
    // public async Task<Usuario?> ResetarSenhaUsuario(string email, string novaSenha)
    // {
    //   var usuario = await _context.Usuario
    //       .FirstOrDefaultAsync(u => u.Email == email);

    //   if (usuario == null)
    //   {
    //     return null;
    //   }

    //   usuario.Senha = novaSenha;
    //   usuario.DataAtualizacao = DateTime.Now;

    //   await _context.SaveChangesAsync();
    //   return usuario;
    // }

    public async Task<int> AddUsuarioAsync(Usuario usuario)
    {
      _context.Usuario.Add(usuario);
      await _context.SaveChangesAsync();
      return usuario.Id;
    }

    public async Task<int> AtualizarUsuarioAsync(Usuario usuario)
    {
      var existingUsuario = await _context.Usuario
          .FirstOrDefaultAsync(u => u.Id == usuario.Id);

      if (existingUsuario == null)
      {
        return 0;
      }

      existingUsuario.Nome = usuario.Nome;
      existingUsuario.Email = usuario.Email;

      await _context.SaveChangesAsync();
      return existingUsuario.Id;
    }
    public async Task<Usuario?> ObterUsuarioPorEmailAsync(string email)
    {
      return await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> RecuperarSenha(string email)
    {


      return await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email == email);
        
    }

    public async Task<bool> ExisteAdminAsync()
    {
      return await _context.Usuario
          .AnyAsync(u => u.Role == "Admin");
    }

    public async Task<bool> PromoverUsuarioAdminAsync(string email)
    {
      var usuario = await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email == email);

      if (usuario == null)
      {
        return false;
      }

      usuario.Role = "Admin";
      await _context.SaveChangesAsync();
      return true;
    }

    //   public async Task<int> DeleteUsuarioAsync(int id)
    //   {
    //     var usuario = await _context.Usuarios
    //         .FirstOrDefaultAsync(u => u.Id == id);

    //     if (usuario == null)
    //     {
    //       return 0;
    //     }

    //     _context.Usuarios.Remove(usuario);
    //     await _context.SaveChangesAsync();
    //     return id;
    //   }
    // }// ajustar para a remoção lógica 
  }
}