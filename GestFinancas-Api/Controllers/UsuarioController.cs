using GestFinancas_Api.Data;
using GestFinancas_Api.Models;
using Microsoft.AspNetCore.Mvc;

using GestFinancas_Api.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using GestFinancas_Api.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System.Linq;


using System.Security.Cryptography;
using GestFinancas_Api.Identity;



namespace GestFinancas.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  
  public class UsuarioController : ControllerBase
  {
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly EnviarEmail _enviarEmail;
    private readonly IAuthenticate _authenticate;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _db;

    public UsuarioController(IUsuarioRepository usuarioRepository, IConfiguration configuration, IAuthenticate authenticate, AppDbContext db)
    {
      _usuarioRepository = usuarioRepository;
      _authenticate = authenticate;
      _configuration = configuration;
      _db = db;
      _enviarEmail = new EnviarEmail(configuration, _usuarioRepository);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Obtém todos os usuários cadastrados.", 
                      Description = "Este endpoint retorna uma lista de todos os usuários cadastrados no sistema.")]
    public async Task<IActionResult> ObterTodosUsuarios()
    {
    
      var usuario = await _usuarioRepository.ObterTodosUsuariosAsync();

      if (usuario.Count == 0)
      {
        return NotFound(new { message = "Nenhum usuário encontrado." });
      }
      var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
      
      return Ok(usuario);
    }


    [HttpPost("login")]
    [SwaggerOperation(Summary = "Realiza o login do usuário.", 
                      Description = "Este endpoint autentica o usuário com email e senha, retornando um token JWT em caso de sucesso.")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var autenticado = await _authenticate.AuthenticateAscync(login.Email, login.Senha);
      if (!autenticado)
        return Unauthorized(new { message = "Email ou senha inválidos." });

      var usuario = await _usuarioRepository.BuscarUsuarioPorEmail(login.Email);
      if (usuario == null)
        return NotFound(new { message = "Usuário não encontrado." });

      var token = await _authenticate.GenerateToken(usuario.Id, usuario.Email);
      var userToken = new UserToken { Token = token };

      return Ok(new { message = "Login realizado com sucesso", data = userToken });
    }

    [HttpPost("cadastrar-usuario")]
    [SwaggerOperation(Summary = "Cadastra um novo usuário.", 
                      Description = "Este endpoint cria um novo usuário no sistema com os dados fornecidos.")]
    public async Task<IActionResult> AddUsuario([FromBody] Usuario usuario)
    {
      if (usuario == null || !usuario.IsValid())
      {
        return BadRequest(new { message = "Dados inválidos." });
      }

      usuario.Role = "User";

      var emailExiste = await _usuarioRepository.BuscarUsuarioPorEmail(usuario.Email);
      if (emailExiste != null)
      {
        return BadRequest(new { message = "Este email já está cadastrado" });
      }
      using var hmac = new HMACSHA512();

      usuario.SenhaSalt = Convert.ToBase64String(hmac.Key);
      usuario.SenhaHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(usuario.Senha)));

      usuario.Senha = null; 

      var usuarioId = await _usuarioRepository.AddUsuarioAsync(usuario);

      if (usuarioId == null)
      {
        return BadRequest(new { message = "Erro ao cadastrar usuário." });
      }

      var token = await _authenticate.GenerateToken(usuario.Id, usuario.Email);
      var userToken = new UserToken { Token = token };

      return Ok(new { message = "Usuário cadastrado com sucesso", data = userToken });
    }

    [HttpPost("promote-admin")]
    [SwaggerOperation(Summary = "Promove um usuário a Admin.",
                      Description = "Este endpoint promove um usuário a Admin. Requer role Admin ou AdminSetupKey quando não houver admin criado.")]
    public async Task<IActionResult> PromoteAdmin([FromBody] PromoteAdminDto dto)
    {
      if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
      {
        return BadRequest(new { message = "Email é obrigatório." });
      }

      var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
      var isAdmin = string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase);

      if (!isAdmin)
      {
        var adminExists = await _usuarioRepository.ExisteAdminAsync();
        if (adminExists)
        {
          return Forbid();
        }

        var setupKey = Environment.GetEnvironmentVariable("ADMIN_SETUP_KEY") ?? _configuration["AdminSetupKey"];
        var providedKey = Request.Headers["X-Admin-Setup-Key"].ToString();

        if (string.IsNullOrEmpty(setupKey) || setupKey != providedKey)
        {
          return Unauthorized(new { message = "AdminSetupKey inválida ou ausente." });
        }
      }

      var promovido = await _usuarioRepository.PromoverUsuarioAdminAsync(dto.Email);
      if (!promovido)
      {
        return NotFound(new { message = "Usuário não encontrado." });
      }

      return Ok(new { message = "Usuário promovido a Admin com sucesso." });
    }

    [Authorize]
    [HttpPut("atualizar-usuario")]
    public async Task<IActionResult> AtualizarUsuario([FromBody] Usuario usuario)
    {
      if (usuario == null || !usuario.IsValid())
      {
        return BadRequest(new { message = "Usuário inválido." });
      }

      var userIdString = User.FindFirst("id")?.Value;
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
        return Unauthorized(new { message = "ID do usuário não encontrado no token." });
      }

      if (!string.Equals(userRole, "Admin", StringComparison.OrdinalIgnoreCase) && usuario.Id != userId)
      {
        return Forbid();
      }

      var usuarioId = await _usuarioRepository.AtualizarUsuarioAsync(usuario);

      if (usuarioId == 0)
      {
        return BadRequest(new { message = "Erro ao atualizar usuário." });
      }

      return Ok(new { message = "Usuário atualizado com sucesso", data = usuarioId });
    }

    [HttpPost("confirmar-reset-senha")] //ainda não usado
    [SwaggerOperation(Summary = "Confirma a redefinição de senha do usuário.", 
                      Description = "Este endpoint redefine a senha do usuário com base no token fornecido.")]
    public async Task<IActionResult> ConfirmarResetSenha([FromBody] RedefinirSenhaTokenDto dto)
    {
      var usuario = await _usuarioRepository.BuscarUsuarioPorToken(dto.Token);
      if (usuario == null || usuario.TokenExpiracao < DateTime.UtcNow)
        return BadRequest(new { message = "Token inválido ou expirado." });

      using var hmac = new HMACSHA512();
      usuario.SenhaSalt = Convert.ToBase64String(hmac.Key);
      usuario.SenhaHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NovaSenha)));

      usuario.Token = null;
      usuario.TokenExpiracao = null;

      var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(usuario);
      if (atualizado == 0)
        return StatusCode(500, new { message = "Erro ao redefinir senha." });

      return Ok(new { message = "Senha redefinida com sucesso." });
    }

    [Authorize]
    [HttpPost("consent")]
    [SwaggerOperation(Summary = "Registra consentimento LGPD",
                      Description = "Registra o consentimento do usuário para termos e política de privacidade.")]
    public async Task<IActionResult> RegistrarConsentimento([FromBody] ConsentDto dto)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var userIdString = User.FindFirst("id")?.Value;
      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
        return Unauthorized(new { message = "ID do usuário não encontrado no token." });
      }

      var consent = new ConsentLog
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        ConsentType = dto.ConsentType,
        Version = dto.Version,
        Accepted = dto.Accepted,
        AcceptedAt = DateTime.UtcNow,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
        UserAgent = Request.Headers["User-Agent"].ToString()
      };

      _db.ConsentLogs.Add(consent);
      await _db.SaveChangesAsync();

      return Ok(new { message = "Consentimento registrado com sucesso." });
    }

    [Authorize]
    [HttpGet("export-data")]
    [SwaggerOperation(Summary = "Exporta dados do usuário",
                      Description = "Exporta dados pessoais, transações e consentimentos do usuário autenticado.")]
    public async Task<IActionResult> ExportarDados()
    {
      var userIdString = User.FindFirst("id")?.Value;
      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
        return Unauthorized(new { message = "ID do usuário não encontrado no token." });
      }

      var usuario = await _db.Usuario.FindAsync(userId);
      if (usuario == null || usuario.IsDeleted)
      {
        return NotFound(new { message = "Usuário não encontrado." });
      }

      var transactions = _db.Transactions.Where(t => t.UserId == userId).ToList();
      var drafts = _db.DraftTransactions.Where(d => d.UserId == userId).ToList();
      var consents = _db.ConsentLogs.Where(c => c.UserId == userId).ToList();

      return Ok(new
      {
        usuario = new
        {
          usuario.Id,
          usuario.Nome,
          usuario.Email,
          usuario.DataCadastro,
          usuario.Role
        },
        transactions,
        drafts,
        consents
      });
    }

    [Authorize]
    [HttpDelete("delete-account")]
    [SwaggerOperation(Summary = "Exclui conta do usuário",
                      Description = "Exclui a conta e anonimiza dados do usuário autenticado.")]
    public async Task<IActionResult> ExcluirConta()
    {
      var userIdString = User.FindFirst("id")?.Value;
      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
        return Unauthorized(new { message = "ID do usuário não encontrado no token." });
      }

      var usuario = await _db.Usuario.FindAsync(userId);
      if (usuario == null)
      {
        return NotFound(new { message = "Usuário não encontrado." });
      }

      if (!usuario.IsDeleted)
      {
        usuario.Nome = "Usuário Excluído";
        usuario.Email = $"deleted_{usuario.Id}@gestfinancas.local";
        usuario.SenhaHash = null;
        usuario.SenhaSalt = null;
        usuario.IsDeleted = true;
        usuario.DeletedAt = DateTime.UtcNow;

        var transactions = _db.Transactions.Where(t => t.UserId == userId);
        var drafts = _db.DraftTransactions.Where(d => d.UserId == userId);
        _db.Transactions.RemoveRange(transactions);
        _db.DraftTransactions.RemoveRange(drafts);

        await _db.SaveChangesAsync();
      }

      return Ok(new { message = "Conta excluída e dados anonimizados." });
    }

    private string GerarToken(Usuario usuario)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "veryverycomplexkey1234567890");
      var identity = new ClaimsIdentity(new Claim[]
      {
       new Claim("role", "user"),
       new Claim(ClaimTypes.Name, $"{usuario.Nome}"),

      });

      var credencials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = identity,
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = credencials
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }
  }
}