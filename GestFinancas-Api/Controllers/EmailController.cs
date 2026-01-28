using Microsoft.AspNetCore.Mvc;
using GestFinancas_Api.Models;
using GestFinancas_Api.Helper;
using GestFinancas_Api.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace GestFinancas_Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class EmailController : ControllerBase
  {
    private readonly EnviarEmail _enviarEmail;
    private readonly IUsuarioRepository _usuarioRepository;


    public EmailController(EnviarEmail enviarEmail, IUsuarioRepository usuarioRepository)
    {
      _enviarEmail = enviarEmail;
      _usuarioRepository = usuarioRepository;
    }

    [HttpPost("email-recuperacao-senha")]
    [SwaggerOperation(Summary = "Envia um email para o usuário com instruções para recuperação de senha.", 
                      Description = "Este endpoint envia um email para o endereço fornecido com um link para redefinir a senha.")]
    public async Task<IActionResult> RecuperarSenha([FromBody] Usuario usuario)
    {
      if (usuario == null || string.IsNullOrEmpty(usuario.Email))
        return BadRequest("O email é obrigatório!");


      await _enviarEmail.EnviarEmailRecuperacaoSenha(usuario.Email);
      return Ok("Email enviado com sucesso!");
    }

    [HttpPost("confirmar-cadastro")]
    [SwaggerOperation(Summary = "Envia um email de confirmação de cadastro para o usuário.", 
                      Description = "Este endpoint envia um email para o endereço fornecido para confirmar o cadastro do usuário.")]
    public async Task<IActionResult> ConfirmarCadastro([FromBody] Usuario usuario)
    {
      if (usuario == null || string.IsNullOrEmpty(usuario.Email))
        return BadRequest("O email é obrigatório!");

      await _enviarEmail.EnviarEmailConfirmacaoCadastro(usuario.Email, usuario.Nome);
      return Ok("Email enviado com sucesso!");
    }

  }
}
