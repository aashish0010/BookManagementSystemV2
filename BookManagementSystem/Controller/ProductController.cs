using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagementSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int companyInfoId = 1,
            [FromQuery] int? categoryId = null,
            [FromQuery] string search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _unitOfWork.productService.GetProducts(companyInfoId, categoryId, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var result = await _unitOfWork.productService.GetProductBySlug(slug);
            if (result.Code == 404)
                return NotFound(result);
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int companyInfoId = 1)
        {
            var result = await _unitOfWork.productService.GetCategories(companyInfoId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            var result = await _unitOfWork.productService.CreateProduct(request);
            return CreatedAtAction(nameof(GetProductBySlug), new { slug = result.Product?.Slug }, result);
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedData([FromQuery] int companyInfoId = 1)
        {
            var result = await _unitOfWork.productService.SeedCategories(companyInfoId);
            if (result.Status == Domain.Entities.Level.Failed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
