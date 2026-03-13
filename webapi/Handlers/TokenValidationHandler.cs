using webapi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

public class TokenValidatorHandler
{
    private readonly AppDbContext _db;

    public TokenValidatorHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task ValidateAsync(TokenValidatedContext context)
    {
        var tokenRaw = context.SecurityToken as JwtSecurityToken;
        var tokenString = tokenRaw?.RawData;

        // Corrige o uso de AnyAsync para IQueryable
        var sessionActive = await _db.UserSessions
            .AnyAsync(s => s.Token == tokenString && !s.IsRevoked);

        if (!sessionActive)
        {
            context.Fail("Acesso negado: Token inexistente ou revogado.");
        }
    }
}