namespace store.server.Infrastructure.Models.Product
{
    public class ProductPricing
    {
        public int? ID { get; set; }
        public int? ProductID { get; set; }
        public int? ColorID { get; set; }
        public int? Amount { get; set; }
    }
}
