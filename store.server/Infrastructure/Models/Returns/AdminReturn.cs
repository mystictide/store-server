namespace store.server.Infrastructure.Models.Returns
{
    public class AdminReturn
    {
        public AdminClient? Client { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class AdminClient
    {
        public int? ID { get; set; }
        public string? Email { get; set; }
    }
}