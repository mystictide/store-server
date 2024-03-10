using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using store.server.Infrastructure.Models.CMS;
using store.server.Infrasructure.Models.Users;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Repo.Main;
using store.server.Infrastructure.Data.Managers.Auth;
using store.server.Infrastructure.Data.Interface.Main;

namespace store.server.Infrastructure.Data.Managers.Main
{
    public class UserManager : AppSettings, IUsers
    {
        private readonly IUsers _repo;
        public UserManager()
        {
            _repo = new UserRepository();
        }

        public async Task<bool> CheckEmail(string Email, int? ID)
        {
            return await _repo.CheckEmail(Email, ID);
        }

        public async Task<Users>? Register(Users entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("Information missing");
            }

            bool userExists = await CheckEmail(entity.Email, null);
            if (userExists)
            {
                throw new Exception("Email address already registered");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;

            var result = await _repo.Register(entity);
            if (result != null)
            {
                var user = new Users();
                user.ID = result.ID;
                user.Email = result.Email;
                user.FirstName = result.FirstName;
                user.LastName = result.LastName;
                user.AccessToken = new TokenManager().GenerateToken(result.ID.Value, 1);
                user.RefreshToken = new TokenManager().GenerateToken(result.ID.Value, 90);
                var tokenEntity = new Tokens { UserID = user.ID.Value, Token = user.RefreshToken, ExpiryDate = DateTime.Now.AddDays(90) };
                await new TokenManager().SaveToken(false, tokenEntity);
                return user;
            }
            throw new Exception("Server error.");
        }

        public async Task<Users>? Login(Users entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("Information missing");
            }

            var result = await _repo.Login(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                var user = new Users();
                user.ID = result.ID;
                user.Email = result.Email;
                user.FirstName = result.FirstName;
                user.LastName = result.LastName;
                user.AccessToken = new TokenManager().GenerateToken(result.ID.Value, 1);
                user.RefreshToken = new TokenManager().GenerateToken(result.ID.Value, 90);
                var tokenEntity = new Tokens { UserID = user.ID.Value, Token = user.RefreshToken, ExpiryDate = DateTime.Now.AddDays(90) };
                await new TokenManager().SaveToken(false, tokenEntity);
                return user;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<Users?> Get(int ID)
        {
            return await _repo.Get(ID);
        }

        public async Task<string?> UpdateEmail(int ID, string Email)
        {
            return await _repo.UpdateEmail(ID, Email);
        }

        public async Task<bool?> ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            var result = await _repo.Get(UserID);
            if (result != null && BCrypt.Net.BCrypt.Verify(currentPassword, result.Password))
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
                return await _repo.ChangePassword(UserID, currentPassword, newPassword);
            }
            return false;
        }

        public async Task<FilteredList<Users>?> FilteredList(Filter filter)
        {
            return await _repo.FilteredList(filter);
        }
    }
}
