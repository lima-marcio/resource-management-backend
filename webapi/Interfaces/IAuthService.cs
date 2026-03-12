namespace webapi.Interfaces
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAndGenerateTokenAsync(string email, string password);
        Task RevokeTokenAsync(string token);
    }
}
