using AutoMapper;
using ECommerceAPI.Infrastructure.Context;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartResponse> GetCart(int userId)
        {
            var cartEntity = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartEntity == null)
            {
                cartEntity = new CartEntity { UserId = userId };
                _context.Carts.Add(cartEntity);
                await _context.SaveChangesAsync();
            }

            return new CartResponse
            {
                CartId = cartEntity.Id,
                Items = cartEntity.CartItems?.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Unknown",
                    ProductImage = ci.Product?.ImageUrl ?? string.Empty,
                    Price = ci.Product?.Price ?? 0,
                    Quantity = ci.Quantity
                }).ToList() ?? new List<CartItemResponse>() // Ensure null-safety
            };
        }
        
        public async Task UpsertCartItem(int userId, CartUpsertRequest request)
        {
            var cartEntity = await GetOrCreateCart(userId);

            var cartItemEntity = cartEntity.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (cartItemEntity != null)
            {
                if (request.Quantity == 0)
                {
                    cartEntity.CartItems.Remove(cartItemEntity);  // ✅ Remove item if quantity is 0
                }
                else
                {
                    cartItemEntity.Quantity = request.Quantity;  // ✅ Update quantity
                }
            }
            else if (request.Quantity > 0)
            {
                cartEntity.CartItems.Add(new CartItemEntity
                {
                    CartId = cartEntity.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }
        
        public async Task ClearCart(int userId)
        {
            var cart = await GetOrCreateCart(userId);
            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
        }
        
        public async Task BulkUpdateCart(int userId, CartBulkUpdateRequest request)
        {
            var cartEntity = await GetOrCreateCart(userId);

            foreach (var item in request.Items)
            {
                var cartItem = cartEntity.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (cartItem != null)
                {
                    if (item.Quantity == 0)
                    {
                        cartEntity.CartItems.Remove(cartItem);  // ✅ Remove item if quantity is 0
                    }
                    else
                    {
                        cartItem.Quantity = item.Quantity;  // ✅ Update quantity
                    }
                }
                else if (item.Quantity > 0)
                {
                    cartEntity.CartItems.Add(new CartItemEntity
                    {
                        CartId = cartEntity.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
        }


        private async Task<CartEntity> GetOrCreateCart(int userId)
        {
            var cartEntity = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartEntity == null)
            {
                cartEntity = new CartEntity { UserId = userId };
                _context.Carts.Add(cartEntity);
                await _context.SaveChangesAsync();
            }

            return cartEntity;
        }
    }
}