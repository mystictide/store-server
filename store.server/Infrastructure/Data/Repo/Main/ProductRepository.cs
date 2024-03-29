﻿using Dapper;
using store.server.Helpers;
using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Models.Product;
using store.server.Infrastructure.Models.Returns;
using store.server.Infrastructure.Data.Repo.Helpers;
using store.server.Infrastructure.Data.Interface.Main;

namespace store.server.Infrastructure.Data.Repo.Main
{
    public class ProductRepository : AppSettings, IProducts
    {
        public async Task<bool> Archive(Products entity)
        {
            try
            {
                string query = $@"
                UPDATE products
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<FilteredList<Products>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Products();
                string categories = "'%%'";
                string brands = "'%%'";
                string materials = "'%%'";
                string colors = "'%%'";
                if (filter.Categories?.Count > 0)
                {
                    categories = CustomHelpers.SeralizeFilterCategories(filter.Categories);
                }
                if (filter.Brands?.Count > 0)
                {
                    brands = CustomHelpers.SeralizeFilterBrands(filter.Brands);
                }
                if (filter.Materials?.Count > 0)
                {
                    materials = CustomHelpers.SeralizeFilterMaterials(filter.Materials);
                }
                if (filter.Colors?.Count > 0)
                {
                    colors = CustomHelpers.SeralizeFilterColors(filter.Colors);
                }
                string category = $@"t.categoryid in (select id from productcategories p where p.name ilike ANY(ARRAY[{categories}]))";
                string brand = $@"t.id in (select productid from productspecifications ps where ps.brandid in (select id from brands b where b.name ilike ANY(ARRAY[{brands}])))";
                string material = $@"t.id in (select productid from productspecifications ps where ps.materialid in (select id from materials b where b.name ilike ANY(ARRAY[{materials}])))";
                string color = $@"t.id in (select productid from productcolors ps where ps.colorid in (select id from colors b where b.name ilike ANY(ARRAY[{colors}])))";
                string PriceRangeMax = filter.PriceRangeMax > 0 ? $"< {filter.PriceRangeMax}" : "notnull";

                FilteredList<Products> request = new FilteredList<Products>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Products> result = new FilteredList<Products>();
                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive} and {category} and {brand} and {material} and {color} and t.id in (select productid from productpricing pp where pp.amount > {filter.PriceRangeMin} and pp.amount {PriceRangeMax})";

                string query_count = $@"Select Count(t.id) from products t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *,
                    (select source from productimages p2 where p2.productid = t.id limit 1) as image,
                    (select amount from productpricing p2 where p2.productid = t.id limit 1) as price,
                    (select name from productcategories p2 where p2.id = t.categoryid) as categoryname
                    FROM products t
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Products>(query);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Products?> Get(int? ID, string? Name)
        {
            try
            {
                string WhereClause;
                if (Name != null)
                {
                    WhereClause = $" WHERE t.id notnull AND (t.name ilike '%{Name}%')";
                }
                else
                {
                    WhereClause = $" WHERE t.id = {ID ?? 0}";
                }
                string query = $@"
                SELECT * FROM products t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Products>(query);
                    res.FirstOrDefault().Specs = new ProductSpecifications();

                    string categoryQuery = $@"
                   SELECT * FROM productcategories t
                   left join productcategories pc on pc.id = t.parentid
                   where t.id in (select categoryid from products p where p.id = {res.FirstOrDefault()?.ID});";

                    string cQuery = $@"
                    SELECT * FROM colors t
                    where t.id in (select colorid from productcolors p where p.productid = {res.FirstOrDefault()?.ID});";

                    string sQuery = $@"
                    SELECT * FROM productspecifications t
                    left join brands b on b.id = t.brandid
                    left join materials m on m.id = t.materialid
                    where t.productid = {res.FirstOrDefault()?.ID};";

                    string iQuery = $@"
                    SELECT * FROM productimages t where t.productid = {res.FirstOrDefault()?.ID};";

                    string stocksQuery = $@"
                    SELECT * FROM productstocks t where t.productid = {res.FirstOrDefault()?.ID};";

                    string pricesQuery = $@"
                    SELECT * FROM productpricing t where t.productid = {res.FirstOrDefault()?.ID};";

                    var specs = await con.QueryAsync<ProductSpecifications, Brands, Materials, ProductSpecifications>(sQuery, (i, b, m) =>
                    {
                        i.Brand = b ?? new Brands();
                        i.Material = m ?? new Materials();
                        return i;
                    }, splitOn: "id");
                    var categories = await con.QueryAsync<ProductCategories, ProductCategories, ProductCategories>(categoryQuery, (i, c) =>
                    {
                        i.Parent = c ?? new ProductCategories();
                        return i;
                    }, splitOn: "id");
                    res.FirstOrDefault().Category = categories.FirstOrDefault();
                    res.FirstOrDefault().Specs = specs.FirstOrDefault();
                    res.FirstOrDefault().Colors = await con.QueryAsync<Colors>(cQuery);
                    res.FirstOrDefault().Images = await con.QueryAsync<ProductImages>(iQuery);
                    res.FirstOrDefault().Stocks = await con.QueryAsync<ProductStocks>(stocksQuery);
                    res.FirstOrDefault().Prices = await con.QueryAsync<ProductPricing>(pricesQuery);
                    return res.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Products?> Manage(Products entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }
                if (entity.Description.Contains("'"))
                {
                    entity.Description = entity.Description.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO products (id, name, description, categoryid, isactive)
	 	        VALUES ({identity}, '{entity.Name}', '{entity.Description}', {entity.Category?.ID}, true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}',
                      categoryid = {entity.Category?.ID},
                      description =  '{entity.Description}'
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Products>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool> ArchiveCategory(ProductCategories entity)
        {
            try
            {
                string query = $@"
                UPDATE productcategories
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<FilteredList<ProductCategories>?> FilteredCategories(Filter filter)
        {
            try
            {
                var filterModel = new ProductCategories();
                FilteredList<ProductCategories> request = new FilteredList<ProductCategories>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<ProductCategories> result = new FilteredList<ProductCategories>();
                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from productcategories t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM productcategories t
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<ProductCategories>(query);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<ProductCategories?> GetCategory(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM productcategories t
                left join productcategories pc on pc.id = t.parentid
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryAsync<ProductCategories, ProductCategories, ProductCategories>(query, (i, u) =>
                        {
                            i.Parent = u ?? new ProductCategories();
                            return i;
                        }, splitOn: "id");
                        return res.FirstOrDefault();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProductCategories>?> GetCategories()
        {
            try
            {
                string query = $@"
                SELECT *
                FROM productcategories t
                left join productcategories pc on pc.id = t.parentid
                WHERE t.isactive = true;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<ProductCategories, ProductCategories, ProductCategories>(query, (i, u) =>
                    {
                        i.Parent = u ?? new ProductCategories();
                        return i;
                    }, splitOn: "id");
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<ProductCategories> ManageCategory(ProductCategories entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                dynamic parent = entity.Parent?.ID > 0 ? entity.Parent.ID : "NULL";
                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }
                string query = $@"
                INSERT INTO productcategories (id, parentid, name, isactive)
	 	        VALUES ({identity}, {parent}, '{entity.Name}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}',
                      parentid = {parent}
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<ProductCategories>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Colors>> ManageColors(List<Colors> entity, int ProductID)
        {
            try
            {
                if (entity.Count > 0)
                {
                    using (var connection = GetConnection)
                    {
                        string deleteExisting = $@"DELETE from productcolors t where t.productid = {ProductID};";
                        await connection.QueryAsync<Colors>(deleteExisting);
                        foreach (var item in entity)
                        {
                            string query = $@"
                            INSERT INTO productcolors (id, productid, colorid)
	 	                    VALUES (default, {ProductID}, {item.ID})
                            RETURNING *;";
                            await connection.QueryAsync<Colors>(query);
                        }
                        return entity;
                    }
                }
                return entity;
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<ProductSpecifications> ManageSpecs(ProductSpecifications entity, int ProductID)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                string query = $@"
                INSERT INTO productspecifications (id, productid, brandid, materialid, height, width, weight)
	 	        VALUES ({identity}, {ProductID}, {entity.Brand?.ID}, {entity.Material?.ID}, {entity.Height}, {entity.Width}, {entity.Weight})
                ON CONFLICT (id) DO UPDATE 
                SET brandid = {entity.Brand?.ID},
                      materialid = {entity.Material?.ID},
                      height =  {entity.Height},
                      width =  {entity.Width},
                      weight =  {entity.Weight}
                RETURNING *;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<ProductSpecifications>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProductImages>> ManageImage(string path, int ProductID)
        {
            try
            {
                string query = $@"
                INSERT INTO productimages (id, productid, source)
	 	        VALUES (default, {ProductID}, '{path}')
                ON CONFLICT (id) DO NOTHING;";
                using (var connection = GetConnection)
                {
                    await connection.QueryAsync<ProductImages>(query);
                    string sQuery = $@"Select * from productimages t where t.productid = {ProductID};";
                    var images = await connection.QueryAsync<ProductImages>(sQuery);
                    return images;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<IEnumerable<ProductImages>> DeleteImage(ProductImages entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                string query = $@"
                DELETE from productimages t where t.id = {entity.ID};
                Select * from productimages t where t.productid = {entity.ProductID};";
                using (var connection = GetConnection)
                {
                    var images = await connection.QueryAsync<ProductImages>(query);
                    return images;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProductStocks>> ManageStocks(ProductStocks entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                string query = $@"
                INSERT INTO productstocks (id, productid, colorid, amount)
	 	        VALUES ({identity}, {entity.ProductID}, {entity.ColorID}, {entity.Amount})
                ON CONFLICT (id) DO UPDATE SET amount = '{entity.Amount}';
                Select * from productstocks t where t.productid = {entity.ProductID};";

                using (var connection = GetConnection)
                {
                    var result = await connection.QueryAsync<ProductStocks>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<ProductPricing>> ManagePricing(ProductPricing entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                string query = $@"
                INSERT INTO productpricing (id, productid, colorid, amount)
	 	        VALUES ({identity}, {entity.ProductID}, {entity.ColorID}, {entity.Amount})
                ON CONFLICT (id) DO UPDATE SET amount = '{entity.Amount}';
                Select * from productpricing t where t.productid = {entity.ProductID};";

                using (var connection = GetConnection)
                {
                    var result = await connection.QueryAsync<ProductPricing>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<bool> ArchiveBrand(Brands entity)
        {
            try
            {
                string query = $@"
                UPDATE brands
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<bool> ArchiveMaterial(Materials entity)
        {
            try
            {
                string query = $@"
                UPDATE materials
                SET isactive = {entity.IsActive} 
                WHERE id = {entity.ID} 
                RETURNING isactive;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<Brands?> GetBrand(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM brands t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Brands>(query);
                        return res;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Materials?> GetMaterial(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM materials t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Materials>(query);
                        return res;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Brands> ManageBrand(Brands entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }
                string query = $@"
                INSERT INTO brands (id, name, isactive)
	 	        VALUES ({identity}, '{entity.Name}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}'
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Brands>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<Materials> ManageMaterial(Materials entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }
                string query = $@"
                INSERT INTO materials (id, name, isactive)
	 	        VALUES ({identity}, '{entity.Name}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}'
                RETURNING id;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Materials>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Brands>?> GetBrands()
        {
            try
            {
                string query = $@"
                SELECT *
                FROM brands t
                WHERE t.isactive = true;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Brands>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Materials>?> GetMaterials()
        {
            try
            {
                string query = $@"
                SELECT *
                FROM materials t
                WHERE t.isactive = true;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Materials>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Colors>?> GetColors()
        {
            try
            {
                string query = $@"SELECT * FROM colors";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Colors>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<FilteredList<Brands>?> FilteredBrands(Filter filter)
        {
            try
            {
                var filterModel = new Brands();
                FilteredList<Brands> request = new FilteredList<Brands>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Brands> result = new FilteredList<Brands>();
                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from brands t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM brands t
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Brands>(query);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<FilteredList<Materials>?> FilteredMaterials(Filter filter)
        {
            try
            {
                var filterModel = new Materials();
                FilteredList<Materials> request = new FilteredList<Materials>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Materials> result = new FilteredList<Materials>();
                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from materials t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
                    FROM materials t
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Materials>(query);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<LandingProducts> GetProductsByMainCategory()
        {
            try
            {
                string livingRoom = $@"
                SELECT t.*, (select source from productimages p2 where p2.productid = t.id limit 1) as image FROM products t
                WHERE t.categoryid in (select id from productcategories p where parentid = 1) order by t.id desc limit 16;";

                string kitchen = $@"
                SELECT t.*, (select source from productimages p2 where p2.productid = t.id limit 1) as image FROM products t
                WHERE t.categoryid in (select id from productcategories p where parentid = 2) order by t.id desc limit 16;";

                string bedroom = $@"
                SELECT t.*, (select source from productimages p2 where p2.productid = t.id limit 1) as image FROM products t
                WHERE t.categoryid in (select id from productcategories p where parentid = 3) order by t.id desc limit 16;";

                string diningRoom = $@"
                SELECT t.*, (select source from productimages p2 where p2.productid = t.id limit 1) as image FROM products t
                WHERE t.categoryid in (select id from productcategories p where parentid = 4) order by t.id desc limit 16;";

                using (var con = GetConnection)
                {
                    var landing = new LandingProducts();
                    landing.LivingRoom = await con.QueryAsync<Products>(livingRoom);
                    landing.Kitchen = await con.QueryAsync<Products>(kitchen);
                    landing.Bedroom = await con.QueryAsync<Products>(bedroom);
                    landing.DiningRoom = await con.QueryAsync<Products>(diningRoom);
                    return landing;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
    }
}
