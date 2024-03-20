using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrasructure.Models.Helpers;
using store.server.Infrastructure.Data.Managers.Main;

namespace store.server.Controllers
{
    [ApiController]
    [Route("filter")]
    public class FilterController : ControllerBase
    {
        [HttpPost]
        [Route("products")]
        public async Task<IActionResult> FilterProducts([FromBody] Filter filter)
        {
            try
            {
                var result = await new ProductManager().FilteredList(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("categories")]
        public async Task<IActionResult> FilterCategories([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().FilteredCategories(filter);
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
        [Route("customers")]
        public async Task<IActionResult> FilterCustomers([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new UserManager().FilteredList(filter);
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
        [Route("brands")]
        public async Task<IActionResult> FilterBrands([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().FilteredBrands(filter);
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
        [Route("materials")]
        public async Task<IActionResult> FilterMaterials([FromBody] Filter filter)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().FilteredMaterials(filter);
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
