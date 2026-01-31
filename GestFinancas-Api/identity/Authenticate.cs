using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using GestFinancas_Api.Data;
using GestFinancas_Api.Models;

namespace GestFinancas_Api.Identity
{
  public class Authenticate : IAuthenticate
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public Authenticate(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task<bool> AuthenticateAscync(string email, string senha)
    {
      if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        return false;

      var usuario = await _context.Usuario
          .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

      if (usuario == null || string.IsNullOrEmpty(usuario.SenhaSalt) || string.IsNullOrEmpty(usuario.SenhaHash))
      {
        return false;
      }

      try
      {

        using var hmac = new HMACSHA512(Convert.FromBase64String(usuario.SenhaSalt));
     
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(senha));
        var storedHash = Convert.FromBase64String(usuario.SenhaHash);
        return computedHash.SequenceEqual(storedHash);
      }
      catch
      {
        
        return false;
      }
    }

    public async Task<bool> usuarioExiste(string email)
    {
      return await _context.Usuario
          .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<string> GenerateToken(int id, string email)
    {
      
      var usuario = await _context.Usuario.FindAsync(id);
      
      var secretKey = _configuration["Jwt:SecretKey"];
      if (string.IsNullOrEmpty(secretKey))
        throw new InvalidOperationException("Jwt:SecretKey não está configurada em appsettings.json");
      
      var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
      var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

      var claims = new List<Claim>
      {
        new Claim("id", id.ToString()),
        new Claim("email", email),
        new Claim("name", usuario?.Nome ?? "Usuário"),
        new Claim(ClaimTypes.Name, usuario?.Nome ?? "Usuário"),
        new Claim(ClaimTypes.Role, usuario?.Role ?? "User"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
      };

      var expiration = DateTime.UtcNow.AddMinutes(10);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: expiration,
          signingCredentials: credentials
      );
      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
