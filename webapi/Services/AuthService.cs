using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Data;
using webapi.Interfaces;
using webapi.Models;

namespace webapi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(UserManager<IdentityUser> userManager, AppDbContext context, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        public async Task<string?> AuthenticateAndGenerateTokenAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                // Se as credenciais falharem, revogamos tokens anteriores deste usuário
                await RevokeAllUserTokens(email);
                return null;
            }

            var token = GenerateJwtToken(user, 2);

            // Salva a sessão no banco
            _context.UserSessions.Add(new UserSession
            {
                UserId = user.Id,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(2),
                IsRevoked = false
            });
            await _context.SaveChangesAsync();

            return token;
        }

        public Task RevokeTokenAsync(string token)
        {
            throw new NotImplementedException();
        }
        private string GenerateJwtToken(IdentityUser user, int hours)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, "Admin"), // Exemplo de regra
                new Claim(ClaimTypes.Role, "Editor")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(hours),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //private string GenerateJwt(IdentityUser user) { /* Lógica de Claims e JWT aqui */ return "token_gerado"; }
        private async Task RevokeAllUserTokens(string email) { /* Lógica de update IsRevoked = true */ }
    }
}
