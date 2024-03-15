using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrastructure.Models.Users;
using store.server.Infrastructure.Data.Managers.Main;

namespace store.server.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("manage/cart")]
        public async Task<IActionResult> ManageCart([FromBody] UserCart entity)
        {
            try
            {
                var UserID = AuthHelpers.CurrentUserID(HttpContext);
                if (UserID > 0)
                {
                    entity.UserID = UserID;
                    var result = await new UserManager().ManageCart(entity);
                    return Ok(result);
                }
                return StatusCode(401, "Access denied");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
