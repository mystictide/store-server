using store.server.Infrastructure.Models.CMS;
using store.server.Infrastructure.Data.Repo.Auth;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Data.Interface.Auth;

namespace store.server.Infrastructure.Data.Managers.Auth
{
    public class AdminManager : AppSettings, IAdmin
    {
        private readonly IAdmin _repo;
        public AdminManager()
        {
            _repo = new AdminRepository();
        }

        public string generatePassword(string password)
        {
            try
            {
                var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
                return hashedPassword;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Admins>? Login(Admins entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("Information missing");
            }

            var result = await _repo.Login(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                var user = new Admins();
                user.ID = result.ID;
                user.Email = result.Email;
                user.AccessToken = new TokenManager().GenerateToken(result.ID.Value, 1);
                user.RefreshToken = new TokenManager().GenerateToken(result.ID.Value, 90);
                var tokenEntity = new Tokens { UserID = user.ID.Value, Token = user.RefreshToken, ExpiryDate = DateTime.Now.AddDays(90) };
                await new TokenManager().SaveToken(true, tokenEntity);
                return user;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<Admins?> Get(int ID)
        {
            return await _repo.Get(ID);
        }
    }
}
