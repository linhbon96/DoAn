using MovieBookingApp.Data; 
using MovieBookingApp.Models.DTOs;
using MovieBookingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TheaterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả các theater
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TheaterDTO>>> GetTheaters()
        {
            var theaters = await _context.Theaters
                .Select(t => new TheaterDTO
                {
                    TheaterId = t.TheaterId,
                    Name = t.Name,
                    Location = t.Location,
                    Rows = t.Rows,
                    Columns = t.Columns
                })
                .ToListAsync();
            
            return Ok(theaters);
        }

        // Lấy thông tin theater theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TheaterDTO>> GetTheater(int id)
        {
            var theater = await _context.Theaters.FindAsync(id);

            if (theater == null)
            {
                return NotFound();
            }

            var theaterDTO = new TheaterDTO
            {
                TheaterId = theater.TheaterId,
                Name = theater.Name,
                Location = theater.Location,
                Rows = theater.Rows,
                Columns = theater.Columns
            };

            return Ok(theaterDTO);
        }

        // Thêm mới theater
        [HttpPost]
        public async Task<ActionResult<Theater>> CreateTheater(TheaterCreateDTO theaterDto)
        {
            var theater = new Theater
            {
                Name = theaterDto.Name,
                Location = theaterDto.Location,
                Rows = theaterDto.Rows,
                Columns = theaterDto.Columns
            };

            _context.Theaters.Add(theater);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTheater), new { id = theater.TheaterId }, theater);
        }

        // Cập nhật theater
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTheater(int id, TheaterCreateDTO theaterDto)
        {
            var theater = await _context.Theaters.FindAsync(id);

            if (theater == null)
            {
                return NotFound();
            }

            theater.Name = theaterDto.Name;
            theater.Location = theaterDto.Location;
            theater.Rows = theaterDto.Rows; // Cập nhật số hàng
            theater.Columns = theaterDto.Columns; // Cập nhật số cột

            _context.Entry(theater).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Xóa theater
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTheater(int id)
        {
            var theater = await _context.Theaters.FindAsync(id);

            if (theater == null)
            {
                return NotFound();
            }

            _context.Theaters.Remove(theater);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
