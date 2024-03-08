using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Main;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Models.Product;
using store.server.Infrastructure.Data.Interface.Main;

namespace store.server.Infrastructure.Data.Managers.Main
{
    public class ProductManager : AppSettings, IProducts
    {
        private readonly IProducts _repo;
        public ProductManager()
        {
            _repo = new ProductRepository();
        }
        public async Task<bool> Archive(Products entity)
        {
            return await _repo.Archive(entity);
        }

        public async Task<FilteredList<Products>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter); ;
        }

        public async Task<Products?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<Products?> Manage(Products entity)
        {
            var result = await _repo.Manage(entity);
            await _repo.ManageColors(entity.Colors.ToList(), result.ID.Value);
            await _repo.ManageSpecs(entity.Specs, result.ID.Value);
            return result;
        }

        public async Task<bool> ArchiveCategory(ProductCategories entity)
        {
            return await _repo.ArchiveCategory(entity);
        }

        public async Task<FilteredList<ProductCategories>?> FilteredCategories(Filter filter)
        {
            return await _repo.FilteredCategories(filter);
        }

        public async Task<ProductCategories?> GetCategory(int ID)
        {
            return await _repo.GetCategory(ID);
        }

        public async Task<IEnumerable<ProductCategories>?> GetCategories()
        {
            return await _repo.GetCategories();
        }

        public async Task<ProductCategories> ManageCategory(ProductCategories entity)
        {
            return await _repo.ManageCategory(entity);
        }

        public async Task<IEnumerable<Colors>> ManageColors(List<Colors> entity, int ProductID)
        {
            return await _repo.ManageColors(entity, ProductID);
        }

        public async Task<ProductSpecifications> ManageSpecs(ProductSpecifications entity, int ProductID)
        {
            return await _repo.ManageSpecs(entity, ProductID);
        }

        public async Task<IEnumerable<ProductImages>> ManageImage(string path, int ProductID)
        {
            return await _repo.ManageImage(path, ProductID);
        }

        public async Task<IEnumerable<ProductImages>> DeleteImage(ProductImages entity)
        {
            return await _repo.DeleteImage(entity);
        }

        public async Task<IEnumerable<ProductStocks>> ManageStocks(Products entity)
        {
            return await _repo.ManageStocks(entity);
        }

        public async Task<bool> ArchiveBrand(Brands entity)
        {
            return await _repo.ArchiveBrand(entity);
        }

        public async Task<Brands?> GetBrand(int ID)
        {
            return await _repo.GetBrand(ID);
        }

        public async Task<Brands> ManageBrand(Brands entity)
        {
            return await _repo.ManageBrand(entity);
        }

        public async Task<IEnumerable<Brands>?> GetBrands()
        {
            return await _repo.GetBrands();
        }

        public async Task<bool> ArchiveMaterial(Materials entity)
        {
            return await _repo.ArchiveMaterial(entity);
        }

        public async Task<Materials?> GetMaterial(int ID)
        {
            return await _repo.GetMaterial(ID);
        }

        public async Task<Materials> ManageMaterial(Materials entity)
        {
            return await _repo.ManageMaterial(entity);
        }

        public async Task<IEnumerable<Materials>?> GetMaterials()
        {
            return await _repo.GetMaterials();
        }
        public async Task<IEnumerable<Colors>?> GetColors()
        {
            return await _repo.GetColors();
        }

        public async Task<FilteredList<Brands>?> FilteredBrands(Filter filter)
        {
            return await _repo.FilteredBrands(filter);
        }

        public async Task<FilteredList<Materials>?> FilteredMaterials(Filter filter)
        {
            return await _repo.FilteredMaterials(filter);
        }
    }
}
