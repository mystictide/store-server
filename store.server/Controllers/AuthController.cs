﻿using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrastructure.Models.CMS;
using store.server.Infrasructure.Models.Users;
using store.server.Infrastructure.Models.Returns;
using store.server.Infrastructure.Data.Managers.Main;
using store.server.Infrastructure.Data.Managers.Auth;

namespace store.server.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("admin/refresh/token")]
        public async Task<IActionResult> RefreshAdminToken([FromBody] string token)
        {
            try
            {
                var data = await new TokenManager().RefreshAdminToken(token);
                if (data != null)
                {
                    var adminData = new AdminReturn
                    {
                        AccessToken = data.AccessToken,
                        RefreshToken = data.RefreshToken
                    };
                    return Ok(adminData);
                }
                return Ok(null);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("refresh/token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            try
            {
                var data = await new TokenManager().RefreshToken(token);
                var userData = new UserReturn();
                userData.AccessToken = data.AccessToken;
                userData.RefreshToken = data.RefreshToken;
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Users entity)
        {
            try
            {
                var data = await new UserManager().Register(entity);
                var userData = new UserReturn { Client = new UserClient { ID = data.ID, Email = data.Email, FirstName = data.FirstName, LastName = data.LastName }, AccessToken = data.AccessToken, RefreshToken = data.RefreshToken };
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Users user)
        {
            try
            {
                var data = await new UserManager().Login(user);
                var userData = new UserReturn { Client = new UserClient { ID = data.ID, Email = data.Email, FirstName = data.FirstName, LastName = data.LastName }, AccessToken = data.AccessToken, RefreshToken = data.RefreshToken };
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("cms/login")]
        public async Task<IActionResult> CMSLogin([FromBody] Admins admin)
        {
            try
            {
                var data = await new AdminManager().Login(admin);
                var adminData = new AdminReturn { Client = new AdminClient { ID = data.ID, Email = data.Email }, AccessToken = data.AccessToken, RefreshToken = data.RefreshToken };
                return Ok(adminData);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("cmail")]
        public async Task<IActionResult> CheckExistingEmail([FromBody] string email)
        {
            try
            {
                bool exists;
                var UserID = AuthHelpers.CurrentUserID(HttpContext);
                if (UserID < 1)
                {
                    exists = await new UserManager().CheckEmail(email, null);
                }
                else
                {
                    exists = await new UserManager().CheckEmail(email, UserID);
                }
                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
