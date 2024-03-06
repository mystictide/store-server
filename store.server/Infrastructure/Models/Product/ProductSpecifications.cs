namespace store.server.Infrastructure.Models.Product
{
    public class ProductSpecifications
    {
        public int? ID { get; set; }
        public int? ProductID { get; set; }
        public Brands? Brand { get; set; }
        public Materials? Material { get; set; }
        public int? Height { get; set; }
        public int? Width { get; set; }
        public int? Weight { get; set; }
    }
}
