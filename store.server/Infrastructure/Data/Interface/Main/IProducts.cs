using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Product;
using store.server.Infrastructure.Models.Returns;

namespace store.server.Infrastructure.Data.Interface.Main
{
    public interface IProducts : IBase<Products>
    {
        Task<bool> ArchiveCategory(ProductCategories entity);
        Task<bool> ArchiveBrand(Brands entity);
        Task<bool> ArchiveMaterial(Materials entity);
        Task<ProductCategories?> GetCategory(int ID);
        Task<Brands?> GetBrand(int ID);
        Task<Materials?> GetMaterial(int ID);
        Task<ProductCategories> ManageCategory(ProductCategories entity);
        Task<Brands> ManageBrand(Brands entity);
        Task<Materials> ManageMaterial(Materials entity);
        Task<FilteredList<ProductCategories>?> FilteredCategories(Filter filter);
        Task<FilteredList<Brands>?> FilteredBrands(Filter filter);
        Task<FilteredList<Materials>?> FilteredMaterials(Filter filter);
        Task<LandingProducts> GetProductsByMainCategory();
        Task<IEnumerable<ProductCategories>?> GetCategories();
        Task<IEnumerable<Brands>?> GetBrands();
        Task<IEnumerable<Materials>?> GetMaterials();
        Task<IEnumerable<Colors>?> GetColors();
        Task<IEnumerable<Colors>> ManageColors(List<Colors> entity, int ProductID);
        Task<ProductSpecifications> ManageSpecs(ProductSpecifications entity, int ProductID);
        Task<IEnumerable<ProductImages>> ManageImage(string path, int ProductID);
        Task<IEnumerable<ProductImages>> DeleteImage(ProductImages entity);
        Task<IEnumerable<ProductStocks>> ManageStocks(ProductStocks entity);
        Task<IEnumerable<ProductPricing>> ManagePricing(ProductPricing entity);
    }
}
