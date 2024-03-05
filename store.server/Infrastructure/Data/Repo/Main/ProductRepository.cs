using Dapper;
using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Models.Product;
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
                FilteredList<Products> request = new FilteredList<Products>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Products> result = new FilteredList<Products>();
                string WhereClause = $@"WHERE t.name ilike '%{filter.Keyword}%' and t.isactive = {filter.IsActive}";
                string query_count = $@"Select Count(t.id) from products t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT *
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

        public async Task<Products?> Get(int ID)
        {
            try
            {
                string query = $@"
                SELECT *
                FROM products t
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<Products>(query);
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
	 	        VALUES ({identity}, {entity.Name}, '{entity.Description}', '{entity.Category?.ID}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}',
                      categoryid = '{entity.Category?.ID}',
                      description =  '{entity.Description}',
                RETURNING *;";

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
                WHERE t.isactive = true;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<ProductCategories>(query);
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

        public async Task<IEnumerable<ProductColors>> ManageColors(List<ProductColors> entity, int ProductID)
        {
            try
            {
                if (entity.Count > 0)
                {
                    using (var connection = GetConnection)
                    {
                        string deleteExisting = $@"DELETE from productcolors t where t.productid = {ProductID};";
                        await connection.QueryAsync<ProductColors>(deleteExisting);
                        foreach (var item in entity)
                        {
                            string query = $@"
                            INSERT INTO productcolors (id, productid, colorid)
	 	                    VALUES (default, {ProductID}, {item.Color.Value})
                            RETURNING *;";
                            await connection.QueryAsync<ProductColors>(query);
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
                INSERT INTO products (id, productid, brandid, material, height, width, weight, depth)
	 	        VALUES ({identity}, {ProductID}, {entity.BrandID}, {entity.Material}, {entity.Height}, {entity.Height}, {entity.Width}, {entity.Weight}, {entity.Depth})
                ON CONFLICT (id) DO UPDATE 
                SET brandid = {entity.BrandID},
                      material = {entity.Material},
                      height =  {entity.Height},
                      width =  {entity.Width},
                      weight =  {entity.Weight},
                      depth =  {entity.Depth}
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

        public async Task<IEnumerable<ProductImages>> ManageImages(List<ProductImages> entity, int ProductID)
        {
            try
            {
                if (entity.Count > 0)
                {
                    using (var connection = GetConnection)
                    {
                        foreach (var item in entity)
                        {
                            dynamic identity = item.ID > 0 ? item.ID : "default";
                            string query = $@"
                            INSERT INTO productimages (id, productid, source)
	 	                    VALUES ({identity}, {ProductID}, '{item.Source}')
                            ON CONFLICT (id) DO NOTHING;";
                            await connection.QueryAsync<ProductImages>(query);
                        }
                        string sQuery = $@"Select * from productimages t where t.productid = {ProductID};";
                        var images = await connection.QueryAsync<ProductImages>(sQuery);
                        return images;
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

        public async Task<IEnumerable<ProductStocks>> ManageStocks(List<ProductStocks> entity, int ProductID)
        {
            try
            {
                if (entity.Count > 0)
                {
                    using (var connection = GetConnection)
                    {
                        string deleteExisting = $@"DELETE from productstocks t where t.productid = {ProductID};";
                        await connection.QueryAsync<ProductStocks>(deleteExisting);
                        foreach (var item in entity)
                        {
                            string query = $@"
                            INSERT INTO productstocks (id, productid, colorid, amount)
	 	                    VALUES (default, {ProductID}, {item.ColorID}, {item.Amount})
                            RETURNING *;";
                            await connection.QueryAsync<ProductStocks>(query);
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
    }
}
