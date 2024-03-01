namespace store.server.Infrastructure.Models.Helpers
{
    public class Tokens
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string? Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
