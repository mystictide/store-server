namespace store.server.Infrastructure.Models.Returns
{
    public class UserReturn
    {
        public UserClient? Client { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
