using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;

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

        public async Task CreateProductAsync(Product product)
        {
            var productEntity = _mapper.Map<ProductEntity>(product);
            await _productRepository.CreateAsync(productEntity);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProductEntity = await _productRepository.GetByIdAsync(product.Id);
            if (existingProductEntity == null) throw new KeyNotFoundException("Product not found");
            // instead of creating new entity, update the tracked entity
            _mapper.Map(product, existingProductEntity);
            await _productRepository.UpdateAsync(_mapper.Map<ProductEntity>(existingProductEntity));
        }

        public async Task DeleteProductAsync(int id)
        {
            var existingProductEntity = await _productRepository.GetByIdAsync(id);
            if (existingProductEntity == null) throw new KeyNotFoundException("Product not found");

            await _productRepository.DeleteAsync(id);
        }
    }
}