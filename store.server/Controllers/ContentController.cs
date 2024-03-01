using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrastructure.Models.Main;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Data.Managers.Main;

namespace store.server.Controllers
{
    [ApiController]
    [Route("cms")]
    public class ContentController : ControllerBase
    {
        [HttpPost]
        [Route("archive/product")]
        public async Task<IActionResult> ArchiveProduct([FromBody] Products entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().Get(entity.ID.Value);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().Archive(entity);
                        return Ok(result);
                    }
                    return StatusCode(401, "Access denied");
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("get/product")]
        public async Task<IActionResult> GetProduct([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().Get(ID);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        return Ok(product);
                    }
                    return StatusCode(401, "Access denied");
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("filter/products")]
        public async Task<IActionResult> FilterProducts([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().FilteredList(filter);
                        return Ok(result);
                    }
                    return StatusCode(401, "Access denied");

                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("manage/product")]
        public async Task<IActionResult> ManageIssue([FromBody] Products entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().Manage(entity);
                        return Ok(result);
                    }
                    return StatusCode(401, "Access denied");
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }
    }
}
