using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrasructure.Models.Users;
using store.server.Infrastructure.Data.Managers.Main;

namespace store.server.Controllers
{
    [ApiController]
    [Route("get")]
    public class GetController : ControllerBase
    {
        [HttpGet]
        [Route("category")]
        public async Task<IActionResult> GetCategory([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetCategory(ID);
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

        [HttpGet]
        [Route("brand")]
        public async Task<IActionResult> GetBrand([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetBrand(ID);
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

        [HttpGet]
        [Route("material")]
        public async Task<IActionResult> GetMaterial([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetMaterial(ID);
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

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await new ProductManager().GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("brands")]
        public async Task<IActionResult> GetBrands()
        {
            try
            {
                var brands = await new ProductManager().GetBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("materials")]
        public async Task<IActionResult> GetMaterials()
        {
            try
            {
                var materials = await new ProductManager().GetMaterials();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("colors")]
        public async Task<IActionResult> GetColors()
        {
            try
            {
                var colors = await new ProductManager().GetColors();
                return Ok(colors);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("product")]
        public async Task<IActionResult> GetProduct([FromQuery] int? ID, [FromQuery] string? Name)
        {
            try
            {
                var product = await new ProductManager().Get(ID, Name);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("landing/products")]
        public async Task<IActionResult> GetProductsByMainCategory()
        {
            try
            {
                var products = await new ProductManager().GetProductsByMainCategory();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpGet]
        [Route("customer")]
        public async Task<IActionResult> GetCustomer([FromQuery] int ID)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var customer = await new UserManager().Get(ID);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        return Ok(customer);
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
        [Route("user/cart")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var user = AuthHelpers.CurrentUserID(HttpContext);
                if (user > 0)
                {
                    var cart = await new UserManager().GetCart(user);
                    return Ok(cart);
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
