using store.server.Infrastructure.Models.CMS;
using store.server.Infrastructure.Models.Returns;

namespace store.server.Infrastructure.Data.Interface.Auth
{
    public interface IAdmin
    {
        Task<Admins>? Login(Admins entity);
        Task<Admins?> Get(int ID);
    }
}
