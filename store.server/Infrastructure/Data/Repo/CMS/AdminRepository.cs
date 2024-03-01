using Dapper;
using store.server.Infrastructure.Models.CMS;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Helpers;
using store.server.Infrastructure.Data.Interface.Auth;

namespace store.server.Infrastructure.Data.Repo.Auth
{
    public class AdminRepository : AppSettings, IAdmin
    {
        public async Task<Admins>? Login(Admins entity)
        {
            try
            {
                string WhereClause = $"WHERE (t.email = '{entity.Email}');";

                string query = $@"
                SELECT *
                FROM admins t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var result = await con.QueryFirstOrDefaultAsync<Admins>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<Admins?> Get(int ID)
        {
            try
            {
                string WhereClause = $" WHERE t.id = {ID}";

                string query = $@"
                SELECT *
                FROM admins t
                {WhereClause};";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Admins>(query);
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
