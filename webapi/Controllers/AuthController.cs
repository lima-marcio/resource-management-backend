using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using webapi.Interfaces;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login-refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] LoginRequest request)
        {
            // Esta rotina valida usuário/senha e gera o token de 2h conforme solicitado
            var token = await _authService.AuthenticateAndGenerateTokenAsync(request.Email, request.Password);

            if (token == null)
                return Unauthorized("Credenciais inválidas. Tokens anteriores revogados.");

            return Ok(new { Token = token, ExpiresIn = "2h" });
        }
    }
}
