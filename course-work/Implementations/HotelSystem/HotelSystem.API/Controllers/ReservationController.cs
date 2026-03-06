using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HotelSystem.API.Data;
using HotelSystem.API.Models;
using HotelSystem.API.DTOs.Reservation;

namespace HotelSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var query = _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .Include(r => r.User)
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var reservations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReservationResponseDto
                {
                    ReservationId = r.ReservationId,
                    RoomId = r.RoomId,
                    HotelName = r.Room.Hotel.Name,
                    UserId = r.UserId,
                    Username = r.User.UserName,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = reservations
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound("Резервацията не е намерена!");

            var dto = new ReservationResponseDto
            {
                ReservationId = reservation.ReservationId,
                RoomId = reservation.RoomId,
                HotelName = reservation.Room.Hotel.Name,
                UserId = reservation.UserId,
                Username = reservation.User.UserName,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                TotalPrice = reservation.TotalPrice,
                Status = reservation.Status
            };

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? status, string? userId, int page = 1, int pageSize = 10)
        {
            var query = _context.Reservations
                .Include(r => r.Room)
                .ThenInclude(r => r.Hotel)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(r => r.UserId == userId);

            var totalItems = await query.CountAsync();

            var reservations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReservationResponseDto
                {
                    ReservationId = r.ReservationId,
                    RoomId = r.RoomId,
                    HotelName = r.Room.Hotel.Name,
                    UserId = r.UserId,
                    Username = r.User.UserName,
                    CheckInDate = r.CheckInDate,
                    CheckOutDate = r.CheckOutDate,
                    TotalPrice = r.TotalPrice,
                    Status = r.Status
                })
                .ToListAsync();

            return Ok(new
            {
                totalItems,
                page,
                pageSize,
                data = reservations
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var room = await _context.Rooms.FindAsync(dto.RoomId);
            if (room == null)
                return BadRequest("Стаята не съществува!");

            if (!room.IsAvailable)
                return BadRequest("Стаята не е свободна!");

            if (dto.CheckInDate >= dto.CheckOutDate)
                return BadRequest("Датата на напускане трябва да е след датата на настаняване!");

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;
            var totalPrice = days * room.PricePerNight;

            var reservation = new Reservation
            {
                RoomId = dto.RoomId,
                UserId = userId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                TotalPrice = totalPrice,
                Status = "Confirmed"
            };

            room.IsAvailable = false;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound("Резервацията не е намерена!");

            if (dto.CheckInDate >= dto.CheckOutDate)
                return BadRequest("Датата на напускане трябва да е след датата на настаняване!");

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;
            reservation.TotalPrice = days * reservation.Room.PricePerNight;

            if (dto.CheckInDate != default) reservation.CheckInDate = dto.CheckInDate;
            if (dto.CheckOutDate != default) reservation.CheckOutDate = dto.CheckOutDate;
            if (!string.IsNullOrEmpty(dto.Status)) reservation.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound("Резервацията не е намерена!");

            reservation.Room.IsAvailable = true;

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return Ok("Резервацията е изтрита успешно!");
        }
    }
}
