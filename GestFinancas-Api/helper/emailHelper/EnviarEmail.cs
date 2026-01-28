using System.Net;
using System.Net.Mail;
using System.Text;
using GestFinancas_Api.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GestFinancas_Api.Data;

namespace GestFinancas_Api.Helper
{
  public class EnviarEmail
  {
    private readonly string _emailRemetente;
    private readonly string _senha;
    private readonly IUsuarioRepository _usuarioRepository;

    public EnviarEmail(IConfiguration configuration, IUsuarioRepository usuarioRepository)
    {
      _emailRemetente = configuration["EmailSettings:EmailRemetente"];
      _senha = configuration["EmailSettings:Senha"];
      _usuarioRepository = usuarioRepository;
    }

    public async Task EnviarEmailRecuperacaoSenha(string email)
    {
      var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(email);

      if (usuario == null)
      {
        Console.WriteLine("Usuário não encontrado!");
        return;
      }

      string nomeUsuario = usuario.Nome;

      if (string.IsNullOrEmpty(email))
      {
        Console.WriteLine("Email não informado!");
        return;
      }


      usuario.Token = Guid.NewGuid().ToString();
      Console.WriteLine($"Token gerado: {usuario.Token}");
      usuario.TokenExpiracao = DateTime.UtcNow.AddHours(24);
      await _usuarioRepository.AtualizarUsuarioAsync(usuario);


      MailMessage mailMessage = new MailMessage(_emailRemetente, email);
      SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

      try
      {
        mailMessage.Subject = "Recuperação de senha";
        mailMessage.IsBodyHtml = true;
        string link = $"http://localhost:4200/recuperar-senha?token={usuario.Token}";
        Console.WriteLine($"Link gerado: {link}");
        mailMessage.Body = $@"
            <h1>Olá, {nomeUsuario}!</h1><hr>
            <p>Recebemos uma solicitação para redefinir sua senha no <strong>GestFinancas</strong>.</p>
            <p>Para criar uma nova senha, clique no link abaixo:</p>
            <p><a href='{link}'>Redefinir Senha</a></p>
            <p>Se você não fez essa solicitação, ignore este e-mail. Sua conta permanecerá segura.</p>
            <p>Este link é válido por 24 horas.</p>
            <hr><p><strong>Equipe GestFinanças</strong></p>";

        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_emailRemetente, _senha);
        smtpClient.EnableSsl = true;

        smtpClient.Send(mailMessage);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Erro ao enviar email: " + ex.Message);
      }
    }

    //vou tentar adicionar a lógica para o envio de email de confirmação de cadastro
    public async Task EnviarEmailConfirmacaoCadastro(string email, string nomeUsuario)
    {
      var usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(email);

      if (string.IsNullOrEmpty(email))
      {
        Console.WriteLine("Email não informado!");
        return;
      }
      
      MailMessage mailMessage = new MailMessage(_emailRemetente, email);
      SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

      try
      {
        mailMessage.Subject = "Confirmação de Cadastro";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = $@"
          <h1>Olá, {nomeUsuario}!</h1><hr>
          <p>Bem-vindo ao <strong>GestFinancas</strong>!</p>
          <p>Seu cadastro foi realizado com sucesso.</p>
          <p>Aproveite para explorar todas as funcionalidades que oferecemos.</p>
          <hr><p><strong>Equipe GestFinanças</strong></p>";

        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(_emailRemetente, _senha);
        smtpClient.EnableSsl = true;

        smtpClient.Send(mailMessage);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Erro ao enviar email: " + ex.Message);
      }
    }
  }
}