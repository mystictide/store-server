namespace store.server.Infrastructure.Models.Returns
{
    public class UserReturn
    {
        public Client? Client { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class Client
    {
        public int? ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? Name
        {
            get { return FirstName + " " + LastName; }
            set { }
        }
    }
}
