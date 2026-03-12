namespace webapi.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; }
    }
}
