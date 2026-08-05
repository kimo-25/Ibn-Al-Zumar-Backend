using IbnAlZumar.API.Common.Exceptions;
using IbnAlZumar.API.DTOs.Catalog;
using IbnAlZumar.API.Services.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace IbnAlZumar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(IProductService productService, IWebHostEnvironment environment)
        {
            _productService = productService;
            _environment = environment;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResultDto<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
        {
            // Accept multiple possible query key names used by various frontends:
            if (string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var q = HttpContext.Request.Query;
                if (q.TryGetValue("search", out StringValues v) && !StringValues.IsNullOrEmpty(v))
                    filter.SearchTerm = v.ToString();
                else if (q.TryGetValue("query", out v) && !StringValues.IsNullOrEmpty(v))
                    filter.SearchTerm = v.ToString();
                else if (q.TryGetValue("keyword", out v) && !StringValues.IsNullOrEmpty(v))
                    filter.SearchTerm = v.ToString();
                else if (q.TryGetValue("name", out v) && !StringValues.IsNullOrEmpty(v))
                    filter.SearchTerm = v.ToString();
            }

            var result = await _productService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Products.Create")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // استخدام الـ SKU كاسم للصورة
                    var extension = Path.GetExtension(imageFile.FileName);
                    var fileName = $"{dto.SKU}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    dto.ImageUrl = $"/uploads/products/{fileName}";
                }
                else
                {
                    dto.ImageUrl = "/uploads/products/default.png";
                }

                var product = await _productService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // استخدام الـ SKU للـ Update لضمان استبدال الصورة القديمة
                    var extension = Path.GetExtension(imageFile.FileName);
                    var fileName = $"{dto.SKU}{extension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    dto.ImageUrl = $"/uploads/products/{fileName}";
                }

                var product = await _productService.UpdateAsync(id, dto);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Products.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}