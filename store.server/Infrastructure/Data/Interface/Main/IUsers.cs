using store.server.Infrasructure.Models.Users;
using store.server.Infrastructure.Models.Users;
using store.server.Infrasructure.Models.Helpers;

namespace store.server.Infrastructure.Data.Interface.Main
{
    public interface IUsers
    {
        Task<bool> CheckEmail(string Email, int? ID);
        Task<Users>? Login(Users entity);
        Task<Users>? Register(Users entity);
        Task<Users?> Get(int ID);
        Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword);
        Task<string?> UpdateEmail(int ID, string Email);
        Task<FilteredList<Users>?> FilteredList(Filter filter);
        Task<IEnumerable<UserCart>?> ManageCart(UserCart entity);
    }
}
