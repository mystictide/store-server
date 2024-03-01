namespace store.server.Infrastructure.Models.CMS
{
    public class Admins
    {
        public int? ID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
