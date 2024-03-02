using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;

namespace store.server.Infrastructure.Data.Interface.Main
{
    public interface IProducts : IBase<Products>
    {
        Task<bool> ArchiveCategory(ProductCategories entity);
        Task<ProductCategories?> GetCategory(int ID);
        Task<ProductCategories> ManageCategory(ProductCategories entity);
        Task<FilteredList<ProductCategories>?> FilteredCategories(Filter filter);
    }
}
