using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class SeatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

    public SeatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get all seats for a specific showtime
    [HttpGet("{showTimeId}")]
public async Task<IActionResult> GetSeats(int showTimeId)
{
    var seats = await _context.Seats
        .Where(s => s.ShowTimeId == showTimeId)
        .ToListAsync();

    if (!seats.Any())
    {
        return NotFound(new { Message = $"No seats found for showtime ID {showTimeId}." });
    }

    // Kiểm tra và cập nhật ghế hết hạn khóa
    foreach (var seat in seats)
    {
        // Kiểm tra nếu ghế đã hết thời gian khóa
        if (seat.LockedUntil.HasValue && seat.LockedUntil <= DateTime.UtcNow)
        {
            // Ghế hết thời gian khóa, mở lại ghế
            seat.IsAvailable = true;  // Đặt lại ghế là có sẵn
            seat.LockedUntil = null;  // Xóa thời gian khóa
        }
    }

    // Lưu lại những thay đổi vào cơ sở dữ liệu
    await _context.SaveChangesAsync();

    var seatDTOs = seats.Select(seat => new SeatDTO
    {
        SeatId = seat.Id,
        Row = seat.Row,
        Number = seat.Number,
        IsAvailable = seat.IsAvailable,  // Khi IsAvailable = false, ghế đã được đặt
        IsLocked = seat.LockedUntil.HasValue && seat.LockedUntil > DateTime.UtcNow,
        LockedUntil = seat.LockedUntil,
        ShowTimeId = seat.ShowTimeId
    }).ToList();

    return Ok(seatDTOs);
}


    // Lock selected seats
    [HttpPost("lock")]
    public async Task<IActionResult> LockSeats([FromBody] LockSeatsRequestDTO request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            return BadRequest(new { Message = "SeatIds list is required." });
        }

        var seatsToLock = await _context.Seats
            .Where(s => request.SeatIds.Contains(s.Id) && s.IsAvailable && (!s.LockedUntil.HasValue || s.LockedUntil <= DateTime.UtcNow))
            .ToListAsync();

        if (!seatsToLock.Any())
        {
            return BadRequest(new { Message = "No seats available to lock. They may already be locked or unavailable." });
        }

        foreach (var seat in seatsToLock)
        {
            seat.LockedUntil = DateTime.UtcNow.Add(LockDuration); // Set lock duration (5 minutes from now)
            seat.IsAvailable = false; // Mark as unavailable during the lock
        }

        await _context.SaveChangesAsync();

        var lockedSeatDTOs = seatsToLock.Select(seat => new SeatDTO
        {
            SeatId = seat.Id,
            Row = seat.Row,
            Number = seat.Number,
            IsAvailable = false,
            IsLocked = true,
            LockedUntil = seat.LockedUntil,
            ShowTimeId = seat.ShowTimeId
        }).ToList();

        return Ok(new { Message = "Seats locked successfully.", LockedSeats = lockedSeatDTOs });
    }

    // Unlock selected seats
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockSeats([FromBody] List<int> seatIds)
    {
        if (seatIds == null || !seatIds.Any())
        {
            return BadRequest(new { Message = "SeatIds list is required." });
        }

        var seats = await _context.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();

        if (!seats.Any())
        {
            return NotFound(new { Message = "None of the requested seats were found to unlock. Check SeatIds." });
        }

        foreach (var seat in seats)
        {
            seat.LockedUntil = null;
            seat.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Seats unlocked successfully.", UnlockedSeats = seatIds });
    }

    // Check seat availability (if they are not locked or reserved)
    [HttpPost("CheckAvailability")]
    public IActionResult CheckAvailability([FromBody] List<int> seatIds)
    {
        if (seatIds == null || !seatIds.Any())
        {
            return BadRequest(new { Message = "Invalid seat list." });
        }

        var unavailableSeats = _context.Seats
            .Where(s => seatIds.Contains(s.Id) && !s.IsAvailable)
            .Select(s => s.Id)
            .ToList();

        if (unavailableSeats.Any())
        {
            return Ok(new { isAvailable = false, unavailableSeats });
        }

        return Ok(new { isAvailable = true });
    }

    // Check if the seats are available for the specific showtime (for reserved tickets)
    [HttpPost("CheckSeats")]
    public async Task<IActionResult> CheckSeats([FromBody] SeatCheckRequest seatCheckDTO)
    {
        if (seatCheckDTO == null || seatCheckDTO.SeatIds == null || seatCheckDTO.SeatIds.Count == 0)
        {
            return BadRequest(new { Message = "Invalid seat data." });
        }

        var unavailableSeats = await _context.Tickets
            .Where(t => t.ShowtimeId == seatCheckDTO.ShowtimeId && seatCheckDTO.SeatIds.Contains(t.SeatId))
            .Select(t => t.SeatId)
            .ToListAsync();

        return Ok(new { UnavailableSeats = unavailableSeats });
    }

    // Lock selected seats when moving to order page
    [HttpPost("lock-and-continue")]
public async Task<IActionResult> LockAndContinue([FromBody] LockSeatsRequestDTO request)
{
    if (request.SeatIds == null || !request.SeatIds.Any())
    {
        return BadRequest(new { Message = "SeatIds list is required." });
    }

    var seatsToLock = await _context.Seats
        .Where(s => request.SeatIds.Contains(s.Id) && s.IsAvailable && (!s.LockedUntil.HasValue || s.LockedUntil <= DateTime.UtcNow))
        .ToListAsync();

    if (!seatsToLock.Any())
    {
        return BadRequest(new { Message = "No seats available to lock. They may already be locked or unavailable." });
    }

    foreach (var seat in seatsToLock)
    {
        seat.LockedUntil = DateTime.UtcNow.Add(LockDuration); // Lock seat for a specified duration
        seat.IsAvailable = false; // Update availability to false (locked)
    }

    await _context.SaveChangesAsync();

    var lockedSeatDTOs = seatsToLock.Select(seat => new SeatDTO
    {
        SeatId = seat.Id,
        Row = seat.Row,
        Number = seat.Number,
        IsAvailable = false,
        IsLocked = true,
        LockedUntil = seat.LockedUntil,
        ShowTimeId = seat.ShowTimeId
    }).ToList();

    return Ok(new { Message = "Seats locked and continue.", LockedSeats = lockedSeatDTOs });
}


    // Auto unlock seats after 5 minutes
   // Thêm thuộc tính HTTP cho phương thức AutoUnlockSeats
[HttpPost("auto-unlock")]
public async Task<IActionResult> AutoUnlockSeats()
{
    var seatsToUnlock = await _context.Seats
        .Where(s => s.LockedUntil.HasValue && s.LockedUntil <= DateTime.UtcNow)
        .ToListAsync();

    if (!seatsToUnlock.Any())
    {
        return Ok(new { Message = "No seats to unlock." });
    }

    foreach (var seat in seatsToUnlock)
    {
        seat.IsAvailable = true;
        seat.LockedUntil = null;
    }

    await _context.SaveChangesAsync();

    return Ok(new { Message = "Seats unlocked successfully.", UnlockedSeats = seatsToUnlock.Select(s => s.Id).ToList() });
}


	
}
