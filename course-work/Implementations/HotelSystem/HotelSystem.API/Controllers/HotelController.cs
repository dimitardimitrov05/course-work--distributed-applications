using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelSystem.API.Data;
using HotelSystem.API.Models;
using HotelSystem.API.DTOs.Hotel;

namespace HotelSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HotelController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Hotels.AsQueryable();

            var totalItems = await query.CountAsync();

            var hotels = await query
                .Include(x => x.Rooms)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = hotels
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var hotel = await _context.Hotels.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.HotelId == id);

            if (hotel == null)
                return NotFound("Хотелът не е намерен!");

            return Ok(hotel);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? name, string? city, int page = 1, int pageSize = 10)
        {
            var query = _context.Hotels.Include(x => x.Rooms).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(h => h.Name.Contains(name));

            if (!string.IsNullOrEmpty(city))
                query = query.Where(h => h.City.Contains(city));

            var totalItems = await query.CountAsync();

            var hotels = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = hotels
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hotel = new Hotel
            {
                Name = dto.Name,
                Address = dto.Address,
                City = dto.City,
                StarRating = dto.StarRating,
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = hotel.HotelId }, hotel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
                return NotFound("Хотелът не е намерен!");

            if (!string.IsNullOrEmpty(dto.Name)) hotel.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Address)) hotel.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.City)) hotel.City = dto.City;
            if (dto.StarRating.HasValue) hotel.StarRating = dto.StarRating.Value;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) hotel.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();

            return Ok(hotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
                return NotFound("Хотелът не е намерен!");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return Ok("Хотелът е изтрит успешно!");
        }
    }
}
