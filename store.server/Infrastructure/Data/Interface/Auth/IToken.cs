using store.server.Infrastructure.Models.Helpers;

namespace store.server.Infrastructure.Data.Interface.Auth
{
    public interface IToken
    {
        Task<Tokens> FindToken(bool admin, string token);
        Task<bool> DeleteToken(bool admin, string token);
        Task<bool> SaveToken(bool admin, Tokens token);
    }
}
