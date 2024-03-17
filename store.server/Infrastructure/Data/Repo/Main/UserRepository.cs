using Dapper;
using store.server.Infrasructure.Models.Users;
using store.server.Infrastructure.Models.Users;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Helpers;
using store.server.Infrastructure.Data.Managers.Main;
using store.server.Infrastructure.Data.Interface.Main;
using store.server.Infrasructure.Models.Users.Helpers;

namespace store.server.Infrastructure.Data.Repo.Main
{
    public class UserRepository : AppSettings, IUsers
    {
        public async Task<bool> CheckEmail(string Email, int? ID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", ID);
            param.Add("@Email", Email);

            string Query;
            if (ID.HasValue)
            {
                Query = $@"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users
                WHERE email = @Email AND NOT (id = @UserID);";
            }
            else
            {
                Query = $@"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users
                WHERE email = @Email;";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }
        public async Task<Users>? Register(Users entity)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                string query = $@"
                INSERT INTO users (firstname, lastname, email, password)
	                VALUES ('{entity.FirstName}', '{entity.LastName}', '{entity.Email}', '{entity.Password}')
                RETURNING *;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = ProcessState.Error;
                return null;
            }
        }
        public async Task<Users>? Login(Users entity)
        {
            try
            {
                string WhereClause = $"WHERE (t.email = '{entity.Email}');";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var result = await con.QueryFirstOrDefaultAsync<Users>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<Users?> Get(int ID)
        {
            try
            {
                string WhereClause = $" WHERE t.id = {ID}";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<string?> UpdateEmail(int ID, string Email)
        {
            try
            {
                var access = await new UserManager().CheckEmail(Email, ID);
                if (!access)
                {
                    string query = $@"
                    UPDATE users
                    SET email = '{Email}'
                    WHERE id = {ID}
                    RETURNING email;";
                    using (var connection = GetConnection)
                    {
                        var res = await connection.QueryFirstOrDefaultAsync<string>(query);
                        return res;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            try
            {
                string query = $@"
                UPDATE users
                SET password = '{newPassword}'
                WHERE id = {UserID};";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync(query);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }

        public async Task<FilteredList<Users>?> FilteredList(Filter filter)
        {
            try
            {
                var filterModel = new Users();
                FilteredList<Users> request = new FilteredList<Users>()
                {
                    filter = filter,
                    filterModel = filterModel,
                };
                FilteredList<Users> result = new FilteredList<Users>();
                string kw = "''";
                if (filter.Keyword != null)
                {
                    kw = $@"'%{filter.Keyword}%'";
                }

                string WhereClause = $@"WHERE TRIM(CONCAT(t.firstname, ' ', t.lastname)) ilike '%{filter.Keyword}%'";
                string query_count = $@"Select Count(t.id) from users t {WhereClause}";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    string query = $@"
                    SELECT t.id, t.firstname, t.lastname, t.email
                    FROM users t
                    {WhereClause}
                    order by t.id {request.filter.SortBy}
                    OFFSET {request.filter.pager.StartIndex} ROWS
                    FETCH NEXT {request.filter.pageSize} ROWS ONLY";
                    result.data = await con.QueryAsync<Users>(query);
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

        public async Task<IEnumerable<UserCart>?> ManageCart(UserCart entity)
        {
            try
            {
                dynamic identity = entity.ID > 0 ? entity.ID : "default";
                string query = $@"
                INSERT INTO usercart (id, userid, productid, colorid, amount)
	 	        VALUES ({identity}, {entity.UserID}, {entity.ProductID}, {entity.ColorID}, {entity.Amount})
                ON CONFLICT (id) DO UPDATE 
                SET amount = {entity.Amount};
                Select * from usercart t where t.userid = {entity.UserID};";

                if (entity.Amount < 1)
                {
                    query = $@"delete from usercart t where t.id = {entity.ID};
                Select * from usercart t where t.userid = {entity.UserID};";
                }

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync<UserCart>(query);
                    return res;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<UserCart>?> GetCart(int UserID)
        {
            try
            {
                string query = $@"Select *,
            (select name from products p where p.id = t.productid)productname,
            (select name from brands b where b.id in (select id from productspecifications ps where ps.id = t.productid))brandname,
            (select hex from colors c where c.id = t.colorid)colorhex,
            (select amount from productpricing pp where pp.productid = t.productid and pp.colorid = t.colorid)pricing, (select source from productimages p2 where p2.productid = t.productid limit 1)image from usercart t where t.userid = {UserID};";
                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync<UserCart>(query);
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