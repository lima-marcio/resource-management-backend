using Microsoft.AspNetCore.Identity;
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

            var token = GenerateJwt(user);

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

        private string GenerateJwt(IdentityUser user) { /* Lógica de Claims e JWT aqui */ return "token_gerado"; }
        private async Task RevokeAllUserTokens(string email) { /* Lógica de update IsRevoked = true */ }
    }
}
