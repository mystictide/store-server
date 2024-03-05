using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Product;

namespace store.server.Infrastructure.Data.Interface.Main
{
    public interface IProducts : IBase<Products>
    {
        Task<bool> ArchiveCategory(ProductCategories entity);
        Task<ProductCategories?> GetCategory(int ID);
        Task<ProductCategories> ManageCategory(ProductCategories entity);
        Task<FilteredList<ProductCategories>?> FilteredCategories(Filter filter);
        Task<IEnumerable<ProductCategories>?> GetCategories();
        Task<IEnumerable<ProductColors>> ManageColors(List<ProductColors> entity, int ProductID);
        Task<ProductSpecifications> ManageSpecs(ProductSpecifications entity, int ProductID);
        Task<IEnumerable<ProductImages>> ManageImages(List<ProductImages> entity, int ProductID);
        Task<IEnumerable<ProductStocks>> ManageStocks(List<ProductStocks> entity, int ProductID);
    }
}
