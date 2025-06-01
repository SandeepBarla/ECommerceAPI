using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.WebApi.DTOs.ResponseModels;

namespace ECommerceAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);
            if (productEntity == null) throw new KeyNotFoundException("Product not found");

            return _mapper.Map<Product>(productEntity);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var productEntities = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Product>>(productEntities);
        }

        public async Task<ProductResponse> GetProductResponseByIdAsync(int id)
        {
            var productEntity = await _productRepository.GetByIdAsync(id);
            if (productEntity == null) throw new KeyNotFoundException("Product not found");

            var productResponse = _mapper.Map<ProductResponse>(productEntity);

            productResponse.CategoryName = productEntity.Category?.Name ?? "Unknown";
            productResponse.SizeName = productEntity.Size?.Name ?? "Unknown";

            return productResponse;
        }

        public async Task<IEnumerable<ProductListResponse>> GetProductListResponsesAsync()
        {
            var productEntities = await _productRepository.GetAllAsync();
            var productListResponses = _mapper.Map<IEnumerable<ProductListResponse>>(productEntities);

            foreach (var response in productListResponses)
            {
                var entity = productEntities.FirstOrDefault(e => e.Id == response.Id);
                if (entity != null)
                {
                    response.CategoryName = entity.Category?.Name ?? "Unknown";
                    response.SizeName = entity.Size?.Name ?? "Unknown";
                }
            }

            return productListResponses;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var productEntity = _mapper.Map<ProductEntity>(product);
            await _productRepository.CreateAsync(productEntity);

            // Ensure the ID is included
            return _mapper.Map<Product>(productEntity);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProductEntity = await _productRepository.GetByIdAsync(product.Id);
            if (existingProductEntity == null) throw new KeyNotFoundException("Product not found");

            // instead of creating new entity, update the tracked entity
            _mapper.Map(product, existingProductEntity);
            await _productRepository.UpdateAsync(existingProductEntity);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }
    }
}