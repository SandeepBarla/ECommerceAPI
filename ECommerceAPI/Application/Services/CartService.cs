using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IUserService userService, IProductService productService,IMapper mapper)
        {
            _cartRepository = cartRepository;
            _userService = userService;
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cartEntity = await GetOrCreateCart(userId);
            var existingItem = cartEntity.CartItems.FirstOrDefault();
            if(existingItem == null) await _cartRepository.SaveCartAsync(cartEntity);
            return _mapper.Map<Cart>(cartEntity);
        }

        public async Task AddOrUpdateCartItemAsync(int userId, CartItem cartItem)
        {
            // ensure product exists
            await _productService.GetProductByIdAsync(cartItem.ProductId);
            // get or create cart
            var cartEntity = await GetOrCreateCart(userId);
            var existingItem = cartEntity.CartItems.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                if (cartItem.Quantity == 0)
                {
                    cartEntity.CartItems.Remove(existingItem);  // Remove item if quantity is 0
                }
                else
                {
                    existingItem.Quantity = cartItem.Quantity;  // Update quantity
                }
            }
            else if (cartItem.Quantity > 0)
            {
                cartEntity.CartItems.Add(_mapper.Map<CartItemEntity>(cartItem));
            }

            await _cartRepository.SaveCartAsync(cartEntity);
        }

        public async Task ClearCartAsync(int userId)
        {
            // ensure user exists
            await _userService.GetUserByIdAsync(userId);
            var cartEntity = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cartEntity == null) throw new KeyNotFoundException("Cart is empty");

            await _cartRepository.RemoveCartAsync(cartEntity);
        }
        
        private async Task<CartEntity> GetOrCreateCart(int userId)
        {
            // ensure user exists
            await _userService.GetUserByIdAsync(userId);
            var cartEntity = await _cartRepository.GetCartByUserIdAsync(userId) ?? new CartEntity { UserId = userId, CartItems = new List<CartItemEntity>() };
            return cartEntity;
        }
    }
}