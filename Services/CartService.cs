using ECommerceAPI.Data;
using ECommerceAPI.DTOs.RequestModels;
using ECommerceAPI.DTOs.ResponseModels;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartResponse> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return new CartResponse
            {
                CartId = cart.Id,
                Items = cart.CartItems.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.ImageUrl,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }
        
        public async Task UpsertCartItem(int userId, CartUpsertRequest request)
        {
            var cart = await GetOrCreateCart(userId);

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

            if (cartItem != null)
            {
                if (request.Quantity == 0)
                {
                    cart.CartItems.Remove(cartItem);  // ✅ Remove item if quantity is 0
                }
                else
                {
                    cartItem.Quantity = request.Quantity;  // ✅ Update quantity
                }
            }
            else if (request.Quantity > 0)
            {
                cart.CartItems.Add(new CartItem
                {
                    CartId = cart.Id,
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
            var cart = await GetOrCreateCart(userId);

            foreach (var item in request.Items)
            {
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (cartItem != null)
                {
                    if (item.Quantity == 0)
                    {
                        cart.CartItems.Remove(cartItem);  // ✅ Remove item if quantity is 0
                    }
                    else
                    {
                        cartItem.Quantity = item.Quantity;  // ✅ Update quantity
                    }
                }
                else if (item.Quantity > 0)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    });
                }
            }

            await _context.SaveChangesAsync();
        }


        private async Task<Cart> GetOrCreateCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}