using Dapper;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Helpers;
using store.server.Infrastructure.Data.Interface.Auth;

namespace store.server.Infrastructure.Data.Repo.Auth
{
    public class TokenRepository : AppSettings, IToken
    {
        public async Task<Tokens> FindToken(bool admin, string token)
        {
            try
            {
                string WhereClause = $" WHERE t.token = '{token}'";
                string query = $@"
                SELECT * FROM {(admin ? "admintokens" : "usertokens")} t {WhereClause};";

                using (var con = GetConnection)
                {
                    var result = await con.QueryFirstOrDefaultAsync<Tokens>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return null;
            }
        }
        public async Task<bool> DeleteToken(bool admin, string token)
        {
            try
            {
                string WhereClause = $" WHERE t.token = '{token}'";
                string query = $@"
                delete FROM  {(admin ? "admintokens" : "usertokens")}  t {WhereClause};";

                using (var con = GetConnection)
                {
                    var result = await con.QueryFirstOrDefaultAsync<Tokens>(query);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }
        public async Task<bool> SaveToken(bool admin, Tokens token)
        {
            try
            {
                string WhereClause = $" WHERE t.token = '{token}'";
                string query = $@"
                SET datestyle = dmy;
                INSERT INTO {(admin ? "admintokens" : "usertokens")} (id, userid, token, expirydate)
	                VALUES (default, '{token.UserID}', '{token.Token}', '{token.ExpiryDate}'::timestamp)
                RETURNING *;";

                using (var con = GetConnection)
                {
                    var existingToken = await FindToken(admin, token.Token);
                    if (existingToken == null)
                    {
                        var tokenResult = await con.QueryFirstOrDefaultAsync<Tokens>(query);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                await new LogsRepository().CreateLog(ex);
                return false;
            }
        }
    }
}
