using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestFinancas_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugTokenController : ControllerBase
    {
        [Authorize]
        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            var userId = User.FindFirst("id")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                message = "Usuário autenticado",
                userId = userId,
                email = email,
                name = name,
                allClaims = claims
            });
        }
    }
}
