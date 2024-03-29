﻿using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using store.server.Infrastructure.Data.Repo.Auth;
using store.server.Infrastructure.Models.Helpers;
using store.server.Infrastructure.Models.Returns;
using store.server.Infrastructure.Data.Interface.Auth;

namespace store.server.Infrastructure.Data.Managers.Auth
{
    public class TokenManager : AppSettings, IToken
    {
        private readonly IToken _repo;
        public TokenManager()
        {
            _repo = new TokenRepository();
        }

        public string GenerateToken(int ID, int days)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(GetSecret());
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                    new Claim("id", ID.ToString()),
                    new Claim("authType", "1")
                }),
                    Expires = DateTime.UtcNow.AddDays(days),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Tokens> FindToken(bool admin, string token)
        {
            return await _repo.FindToken(admin, token);
        }

        public async Task<bool> DeleteToken(bool admin, string token)
        {
            return await _repo.DeleteToken(admin, token);
        }

        public async Task<bool> SaveToken(bool admin, Tokens token)
        {
            return await _repo.SaveToken(admin, token);
        }

        public async Task<AdminReturn> RefreshAdminToken(string refreshToken)
        {
            var existing = await FindToken(true, refreshToken);
            if (existing != null)
            {
                if (DateTime.Now > existing.ExpiryDate)
                {
                    await DeleteToken(true, refreshToken);
                    return null;
                }
                else
                {
                    var admin = new AdminReturn();
                    admin.AccessToken = GenerateToken(existing.UserID, 1);
                    admin.RefreshToken = existing.Token;
                    return admin;
                }
            }
            else
            {
                return null;
            }
        }     
        public async Task<UserReturn> RefreshToken(string refreshToken)
        {
            var existing = await FindToken(false, refreshToken);
            if (DateTime.Now > existing.ExpiryDate)
            {
                await DeleteToken(false, refreshToken);
                return null;
            }
            else
            {
                var user = new UserReturn();
                user.AccessToken = GenerateToken(existing.UserID, 1);
                user.RefreshToken = existing.Token;
                return user;
            }
        }
    }
}
