using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelSystem.API.Data;
using HotelSystem.API.Models;
using HotelSystem.API.DTOs.Room;

namespace HotelSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Rooms.AsQueryable();

            var totalItems = await query.CountAsync();

            var rooms = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = rooms
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound("Стаята не е намерена!");

            return Ok(room);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? roomType, bool? isAvailable, int page = 1, int pageSize = 10)
        {
            var query = _context.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(roomType))
                query = query.Where(r => r.RoomType.Contains(roomType));

            if (isAvailable.HasValue)
                query = query.Where(r => r.IsAvailable == isAvailable.Value);

            var totalItems = await query.CountAsync();

            var rooms = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = rooms
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId);
            if (!hotelExists)
                return BadRequest("Хотелът не съществува!");

            var room = new Room
            {
                HotelId = dto.HotelId,
                RoomType = dto.RoomType,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity,
                IsAvailable = dto.IsAvailable
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = room.RoomId }, room);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoomUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Стаята не е намерена!");

            if (!string.IsNullOrEmpty(dto.RoomType)) room.RoomType = dto.RoomType;
            if (dto.PricePerNight.HasValue) room.PricePerNight = dto.PricePerNight.Value;
            if (dto.Capacity != 0) room.Capacity = dto.Capacity;
            room.IsAvailable = dto.IsAvailable;

            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                return NotFound("Стаята не е намерена!");

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Стаята е изтрита успешно!");
        }
    }
}
