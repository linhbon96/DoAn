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

        // Auto-update expired locks
        foreach (var seat in seats)
        {
            if (seat.LockedUntil.HasValue && seat.LockedUntil <= DateTime.UtcNow)
            {
                seat.IsAvailable = true;
                seat.LockedUntil = null; // Clear expired locks
            }
        }
        await _context.SaveChangesAsync();

        var seatDTOs = seats.Select(seat => new SeatDTO
        {
            SeatId = seat.Id,
            Row = seat.Row,
            Number = seat.Number,
            IsAvailable = !seat.LockedUntil.HasValue || seat.LockedUntil <= DateTime.UtcNow,
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
            seat.LockedUntil = DateTime.UtcNow.Add(LockDuration); // Lock seat for a specified duration
            seat.IsAvailable = false; // Update availability
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
    [HttpPost("CheckSeats")]
    public async Task<IActionResult> CheckSeats([FromBody] SeatCheckDTO seatCheckDTO)
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


}
