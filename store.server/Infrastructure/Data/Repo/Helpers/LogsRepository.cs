﻿using Dapper;
using System.Diagnostics;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Interface.Helpers;

namespace store.server.Infrastructure.Data.Repo.Helpers
{
    public class LogsRepository : AppSettings, ILogs
    {
        public async Task<int> Add(Logs entity)
        {
            try
            {
                string query = $@"
                SET datestyle = dmy;
                INSERT INTO logs (userid, message, source, line, createddate)
	                VALUES ({entity.UserID},'{entity.Message}', '{entity.Source}', {entity.Line}, '{entity.CreatedDate}'::timestamp);";
                using (var con = GetConnection)
                {
                    return await con.ExecuteAsync(query);
                }
            }
            catch (Exception exception)
            {
                string a = exception.Message;
                return 0;
            }
        }

        public async Task<bool> CreateLog(Exception ex, int UserId = 0)
        {
            try
            {
                var st = new StackTrace(ex, true);
                if (st != null)
                {
                    st.GetFrames().Where(k => k.GetFileLineNumber() > 0).ToList().ForEach(async k =>
                    {
                        await new LogsRepository().Add(new Logs()
                        {
                            CreatedDate = DateTime.Now,
                            UserID = UserId,
                            Message = ex.Message,
                            Source = ex.Source + " | " + k,
                            Line = k.GetFileLineNumber()
                        });
                    });
                }
                else
                {
                    await new LogsRepository().Add(new Logs()
                    {
                        CreatedDate = DateTime.Now,
                        UserID = UserId,
                        Message = ex.Message,
                        Source = ex.Source,
                        Line = 0
                    });
                }
            }
            catch (Exception exception)
            {
                await new LogsRepository().Add(new Logs()
                {
                    CreatedDate = DateTime.Now,
                    UserID = 0,
                    Message = exception.Message,
                    Source = exception.Source + " - Error logging",
                    Line = 0
                });
            }
            return true;
        }
    }
}
