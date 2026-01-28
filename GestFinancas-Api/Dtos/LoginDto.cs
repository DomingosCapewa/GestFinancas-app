using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Security.Claims;

namespace GestFinancas_Api.Dtos
{
  public class LoginDto
  {
    [Required]
    public string Email { get; set; }

    [Required]
    public string Senha { get; set; }
  }
}