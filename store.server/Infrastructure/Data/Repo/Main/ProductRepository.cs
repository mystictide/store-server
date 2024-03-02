using Dapper;
using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
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
                INSERT INTO issues (id, name, description, categoryid, isactive)
	 	        VALUES ({identity}, {entity.Name}, '{entity.Description}', '{entity.Category?.ID}', true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}',
                      categoryid = '{entity.Category?.ID}',
                      description =  '{entity.Description}',
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
                WHERE t.id = {ID};";

                using (var con = GetConnection)
                {
                    if (ID > 0)
                    {
                        var res = await con.QueryFirstOrDefaultAsync<ProductCategories>(query);
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

        public async Task<ProductCategories> ManageCategory(ProductCategories entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                if (entity.Name.Contains("'"))
                {
                    entity.Name = entity.Name.Replace("'", "''");
                }

                string query = $@"
                INSERT INTO issues (id, name, isactive)
	 	        VALUES ({identity}, {entity.Name}, true)
                ON CONFLICT (id) DO UPDATE 
                SET name = '{entity.Name}'
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
    }
}
