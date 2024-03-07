﻿using store.server.Infrastructure.Models.Product;

namespace store.server.Infrastructure.Models.Main
{
    public class Products
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ProductCategories? Category { get; set; }
        public IEnumerable<ProductImages>? Images { get; set; }
        public IEnumerable<Colors>? Colors { get; set; }
        public IEnumerable<ProductStocks>? Stocks { get; set; }
        public ProductSpecifications? Specs { get; set; }
        public bool IsActive { get; set; }
    }
}
