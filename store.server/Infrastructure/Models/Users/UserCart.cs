namespace store.server.Infrastructure.Models.Users
{
    public class UserCart
    {
        public int? ID { get; set; }
        public int? UserID { get; set; }
        public int? ProductID { get; set; }
        public int? ColorID { get; set; }
        public int? Amount { get; set; }
        public string? ProductName { get; set; }
        public string? BrandName { get; set; }
        public string? ColorHex { get; set; }
        public decimal? Pricing { get; set; }
    }
}
