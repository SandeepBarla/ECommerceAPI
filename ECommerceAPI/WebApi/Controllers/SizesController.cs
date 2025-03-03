using AutoMapper;
using ECommerceAPI.Application.Interfaces;
using ECommerceAPI.Application.Models;
using ECommerceAPI.WebApi.DTOs.RequestModels;
using ECommerceAPI.WebApi.DTOs.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.WebApi.Controllers
{
    [Route("api/sizes")]
    [ApiController]
    public class SizesController : ControllerBase
    {
        private readonly ISizeService _sizeService;
        private readonly IMapper _mapper;

        public SizesController(ISizeService sizeService, IMapper mapper)
        {
            _sizeService = sizeService;
            _mapper = mapper;
        }

        // ✅ Create Size
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSize([FromBody] SizeUpsertRequest upsertRequest)
        {
            var size = await _sizeService.CreateSizeAsync(_mapper.Map<Size>(upsertRequest));
            var response = _mapper.Map<SizeResponse>(size);
            return CreatedAtAction(nameof(GetSizeById), new { id = response.Id }, response);
        }

        // ✅ Get All Sizes
        [HttpGet]
        public async Task<IActionResult> GetAllSizes()
        {
            var sizes = await _sizeService.GetAllSizesAsync();
            var response = _mapper.Map<List<SizeResponse>>(sizes);
            return Ok(response);
        }

        // ✅ Get Size by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(int id)
        {
            var size = await _sizeService.GetSizeByIdAsync(id);
            var response = _mapper.Map<SizeResponse>(size);
            return Ok(response);
        }

        // ✅ Update Size
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSize(int id, [FromBody] SizeUpsertRequest upsertRequest)
        {
            await _sizeService.UpdateSizeAsync(_mapper.Map<Size>((id, upsertRequest)));
            return NoContent(); // 204 No Content
        }

        // ✅ Delete Size
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSize(int id)
        {
            await _sizeService.DeleteSizeAsync(id);
            return NoContent(); // 204 No Content
        }
    }
}