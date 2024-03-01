using store.server.Infrastructure.Models.Product;

namespace store.server.Infrastructure.Models.Main
{
    public class Products
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ProductCategories? Category { get; set; }
        public ProductColors? Colors { get; set; }
        public ProductSizes? Sizes { get; set; }
        public ProductStocks? Stocks { get; set; }
        public bool IsActive { get; set; }
    }
}
