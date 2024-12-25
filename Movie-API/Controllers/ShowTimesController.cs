using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using MovieBookingApp.Data;
using System.Collections.Generic;
using System;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ShowtimesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy danh sách showtime theo movieId
    [HttpGet("{movieId}")]
    public async Task<IActionResult> GetShowtimes(int movieId)
    {
        var showtimes = await _context.ShowTimes
            .Where(st => st.MovieId == movieId && st.IsActive) // Chỉ lấy các showtime đang hoạt động
            .Include(st => st.Theater)
            .Include(st => st.Movie)
            .Select(st => new ShowtimeDTO
            {
                ShowtimeId = st.ShowtimeId,
                MovieId = st.MovieId,
                MovieTitle = st.Movie.Title,
                TheaterId = st.TheaterId,
                TheaterName = st.Theater.Name,
                ShowDate = st.ShowDate,
                ShowHour = st.ShowHour
            })
            .ToListAsync();

        return Ok(showtimes);
    }

    // Thêm showtime mới và tự động tạo ghế dựa trên Theater
    [HttpPost]
    public async Task<IActionResult> CreateShowtime([FromBody] ShowtimeCreateDTO showtimeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var showtime = new Showtime
        {
            MovieId = showtimeDto.MovieId,
            TheaterId = showtimeDto.TheaterId,
            ShowDate = DateTime.SpecifyKind(showtimeDto.ShowDate, DateTimeKind.Utc),
            ShowHour = showtimeDto.ShowHour,
            IsActive = showtimeDto.IsActive
        };

        _context.ShowTimes.Add(showtime);
        await _context.SaveChangesAsync();

        await InitializeSeatsForShowtime(showtime.TheaterId, showtime.ShowtimeId);

        return CreatedAtAction(nameof(GetShowtimes), new { movieId = showtime.MovieId }, showtime);
    }

    // Khởi tạo ghế dựa trên số hàng và cột của phòng chiếu
    private async Task InitializeSeatsForShowtime(int theaterId, int showtimeId)
    {
        var theater = await _context.Theaters.FindAsync(theaterId);
        if (theater == null) return;

        // Kiểm tra xem đã có ghế cho lịch chiếu này chưa
        bool seatsExist = await _context.Seats.AnyAsync(s => s.ShowTimeId == showtimeId);
        if (seatsExist) return; // Nếu đã có ghế, không tạo thêm

        var seats = new List<Seat>();

        for (int row = 0; row < theater.Rows; row++)
        {
            for (int col = 1; col <= theater.Columns; col++)
            {
                seats.Add(new Seat
                {
                    Row = ((char)('A' + row)).ToString(),
                    Number = col,
                    IsAvailable = true,
                    ShowTimeId = showtimeId
                });
            }
        }

        _context.Seats.AddRange(seats);
        await _context.SaveChangesAsync();
    }

    // Cập nhật showtime theo showtimeId
    [HttpPut("{showtimeId}")]
    public async Task<IActionResult> UpdateShowtime(int showtimeId, [FromBody] ShowtimeCreateDTO showtimeDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var showtime = await _context.ShowTimes.FindAsync(showtimeId);
        if (showtime == null)
        {
            return NotFound();
        }

        showtime.MovieId = showtimeDto.MovieId;
        showtime.TheaterId = showtimeDto.TheaterId;
        showtime.ShowDate = DateTime.SpecifyKind(showtimeDto.ShowDate, DateTimeKind.Utc);
        showtime.ShowHour = showtimeDto.ShowHour;
        showtime.IsActive = showtimeDto.IsActive;

        _context.ShowTimes.Update(showtime);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Xóa showtime theo showtimeId
    [HttpDelete("{showtimeId}")]
    public async Task<IActionResult> DeleteShowtime(int showtimeId)
    {
        var showtime = await _context.ShowTimes.FindAsync(showtimeId);
        if (showtime == null)
        {
            return NotFound();
        }
        _context.ShowTimes.Remove(showtime);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Tự động ẩn lịch chiếu quá hạn
    [HttpPost("auto-hide")]
    public async Task<IActionResult> AutoHideExpiredShowtimes()
    {
        var now = DateTime.UtcNow;
        var expiredShowtimes = await _context.ShowTimes
            .Where(st => st.ShowDate < now.Date || (st.ShowDate == now.Date && st.ShowHour < now.TimeOfDay))
            .ToListAsync();

        foreach (var showtime in expiredShowtimes)
        {
            showtime.IsActive = false;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Expired showtimes hidden successfully." });
    }
}

