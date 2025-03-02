using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.Infrastructure.Entities;
using ECommerceAPI.Infrastructure.Interfaces;

namespace ECommerceAPI.Application.Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IMapper _mapper;

        public SizeService(ISizeRepository sizeRepository, IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _mapper = mapper;
        }

        // ✅ Create Size
        public async Task<Size> CreateSizeAsync(string name)
        {
            var sizeEntity = new SizeEntity
            {
                Name = name
            };
            await _sizeRepository.CreateAsync(sizeEntity);
            return _mapper.Map<Size>(sizeEntity);
        }

        // ✅ Get All Sizes
        public async Task<IEnumerable<Size>> GetAllSizesAsync()
        {
            var sizeEntities = await _sizeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<Size>>(sizeEntities);
        }

        // ✅ Get Size by ID
        public async Task<Size> GetSizeByIdAsync(int id)
        {
            var sizeEntity = await _sizeRepository.GetByIdAsync(id);
            if (sizeEntity == null)
                throw new KeyNotFoundException("Size not found");
            return _mapper.Map<Size>(sizeEntity);
        }

        // ✅ Update Size
        public async Task UpdateSizeAsync(int id, string name)
        {
            var sizeEntity = await _sizeRepository.GetByIdAsync(id);
            if (sizeEntity == null)
                throw new KeyNotFoundException("Size not found");

            sizeEntity.Name = name;
            await _sizeRepository.UpdateAsync(sizeEntity);
        }

        // ✅ Delete Size
        public async Task DeleteSizeAsync(int id)
        {
            var sizeEntity = await _sizeRepository.GetByIdAsync(id);
            if (sizeEntity == null)
                throw new KeyNotFoundException("Size not found");

            await _sizeRepository.DeleteAsync(id);
        }
    }
}