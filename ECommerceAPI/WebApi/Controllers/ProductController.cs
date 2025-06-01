using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using FluentValidation;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly IValidator<ProductUpsertRequest> _productUpsertRequestValidator;

        public ProductController(IMapper mapper, IProductService productService
        , IValidator<ProductUpsertRequest> productUpsertRequestValidator)
        {
            _mapper = mapper;
            _productService = productService;
            _productUpsertRequestValidator = productUpsertRequestValidator;
        }

        // Create Product (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductUpsertRequest productCreateRequest)
        {
            await _productUpsertRequestValidator.ValidateAndThrowAsync(productCreateRequest);
            var product = _mapper.Map<Product>(productCreateRequest);
            var createdProduct = await _productService.CreateProductAsync(product);

            var response = await _productService.GetProductResponseByIdAsync(createdProduct.Id);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, response);
        }

        // Get All Products (Public)
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetProductListResponsesAsync();
            return Ok(response);
        }

        // Get Product by ID (Public)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetProductResponseByIdAsync(id);
            return Ok(response);
        }

        // Update Product (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpsertRequest productUpdateRequest)
        {
            await _productUpsertRequestValidator.ValidateAndThrowAsync(productUpdateRequest);
            var product = _mapper.Map<Product>((id, productUpdateRequest));
            await _productService.UpdateProductAsync(product);

            var response = await _productService.GetProductResponseByIdAsync(id);
            return Ok(response);
        }

        // Delete Product (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Product deleted successfully." });
        }
    }
}