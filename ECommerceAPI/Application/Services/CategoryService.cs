using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // ✅ Create Category
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var categoryEntity = _mapper.Map<CategoryEntity>(category);
            await _categoryRepository.CreateAsync(categoryEntity);
            return _mapper.Map<Category>(categoryEntity);
        }

        // ✅ Get All Categories
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categoryEntities = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Category>>(categoryEntities);
        }

        // ✅ Get Category by ID
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var categoryEntity = await _categoryRepository.GetByIdAsync(id);
            if (categoryEntity == null)
                throw new KeyNotFoundException("Category not found");
            return _mapper.Map<Category>(categoryEntity);
        }

        // ✅ Update Category
        public async Task UpdateCategoryAsync(Category category)
        {
            var categoryEntity = await _categoryRepository.GetByIdAsync(category.Id);
            if (categoryEntity == null)
                throw new KeyNotFoundException("Category not found");

            _mapper.Map(category, categoryEntity);
            await _categoryRepository.UpdateAsync(categoryEntity);
        }

        // ✅ Delete Category
        public async Task DeleteCategoryAsync(int id)
        {
            var categoryEntity = await _categoryRepository.GetByIdAsync(id);
            if (categoryEntity == null)
                throw new KeyNotFoundException("Category not found");

            await _categoryRepository.DeleteAsync(id);
        }
    }
}