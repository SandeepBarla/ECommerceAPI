using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        // ✅ Create Category
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryUpsertRequest upsertRequest)
        {
            var category = await _categoryService.CreateCategoryAsync(_mapper.Map<Category>(upsertRequest));
            var response = _mapper.Map<CategoryResponse>(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = response.Id }, response);
        }

        // ✅ Get All Categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var response = _mapper.Map<List<CategoryResponse>>(categories);
            return Ok(response);
        }

        // ✅ Get Category by ID
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            var response = _mapper.Map<CategoryResponse>(category);
            return Ok(response);
        }

        // ✅ Update Category
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpsertRequest upsertRequest)
        {
            var category = _mapper.Map<Category>((id, upsertRequest));
            await _categoryService.UpdateCategoryAsync(category);
            return NoContent(); // 204 No Content
        }

        // ✅ Delete Category
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return NoContent(); // 204 No Content
        }
    }
}