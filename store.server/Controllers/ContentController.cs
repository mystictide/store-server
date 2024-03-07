using store.server.Helpers;
using Microsoft.AspNetCore.Mvc;
using store.server.Infrastructure.Helpers;
using store.server.Infrastructure.Models.Main;
using store.server.Infrastructure.Models.Product;
using store.server.Infrastructure.Data.Managers.Main;

namespace store.server.Controllers
{
    [ApiController]
    [Route("cms")]
    public class ContentController : ControllerBase
    {
        private IWebHostEnvironment _env;

        public ContentController(IWebHostEnvironment env)
        {
            _env = env;
        }

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

        [HttpPost]
        [Route("manage/product")]
        public async Task<IActionResult> ManageProduct([FromBody] Products entity)
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

        [HttpPost]
        [Route("manage/image")]
        public async Task<IActionResult> ManageImage([FromForm] IFormFile file)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    if (file.Length > 0)
                    {
                        var path = await CustomHelpers.SaveImage(Int32.Parse(file.FileName), _env.ContentRootPath, file);
                        if (path != null)
                        {
                            var result = await new ProductManager().ManageImage(path, Int32.Parse(file.FileName));
                            return Ok(result);
                        }
                        else
                        {
                            return StatusCode(401, "Failed to save image");
                        }
                    }
                    return StatusCode(401, "Failed to save image");
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("delete/image")]
        public async Task<IActionResult> DeleteImage([FromBody] ProductImages entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var res = await CustomHelpers.DeleteImage(entity, _env.ContentRootPath);
                    if (res)
                    {
                        var result = await new ProductManager().DeleteImage(entity);
                        return Ok(result);
                    }
                    else
                    {
                        return StatusCode(401, "Failed to delete image");
                    }
                }
                return StatusCode(401, "Authorization failed");
            }
            catch (Exception ex)
            {
                return StatusCode(401, ex.Message);
            }
        }

        [HttpPost]
        [Route("archive/category")]
        public async Task<IActionResult> ArchiveCategory([FromBody] ProductCategories entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetCategory(entity.ID.Value);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ArchiveCategory(entity);
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
        [Route("archive/brand")]
        public async Task<IActionResult> ArchiveBrand([FromBody] Brands entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetBrand(entity.ID);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ArchiveBrand(entity);
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
        [Route("archive/material")]
        public async Task<IActionResult> ArchiveMaterial([FromBody] Materials entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var product = await new ProductManager().GetMaterial(entity.ID);
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ArchiveMaterial(entity);
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
        [Route("manage/category")]
        public async Task<IActionResult> ManageCategory([FromBody] ProductCategories entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ManageCategory(entity);
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
        [Route("manage/brand")]
        public async Task<IActionResult> ManageBrand([FromBody] Brands entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ManageBrand(entity);
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
        [Route("manage/material")]
        public async Task<IActionResult> ManageMaterial([FromBody] Materials entity)
        {
            try
            {
                if (AuthHelpers.Authorize(HttpContext, 1))
                {
                    var admin = AuthHelpers.CurrentUserID(HttpContext);
                    if (admin > 0)
                    {
                        var result = await new ProductManager().ManageMaterial(entity);
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
