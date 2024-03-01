using store.server.Infrasructure.Models.Helpers;

namespace store.server.Infrastructure.Data.Interface.Helpers
{
    public interface ILogs
    {
        Task<int> Add(Logs entity);
    }
}
