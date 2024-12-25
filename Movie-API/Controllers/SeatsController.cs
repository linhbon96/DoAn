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

    // Lấy tất cả ghế cho một suất chiếu cụ thể
    [HttpGet("{showTimeId}")]
    public async Task<IActionResult> GetSeats(int showTimeId)
    {
        var seats = await _context.Seats
            .Where(s => s.ShowTimeId == showTimeId)
            .ToListAsync();

        if (!seats.Any())
        {
            return NotFound(new { Message = $"Không tìm thấy ghế cho suất chiếu ID {showTimeId}." });
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

    // Khóa các ghế đã chọn
    [HttpPost("lock")]
    public async Task<IActionResult> LockSeats([FromBody] LockSeatsRequestDTO request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            return BadRequest(new { Message = "Danh sách SeatIds là bắt buộc." });
        }

        var seatsToLock = await _context.Seats
            .Where(s => request.SeatIds.Contains(s.Id) && s.IsAvailable && (!s.LockedUntil.HasValue || s.LockedUntil <= DateTime.UtcNow))
            .ToListAsync();

        if (!seatsToLock.Any())
        {
            return BadRequest(new { Message = "Không có ghế nào có sẵn để khóa. Chúng có thể đã bị khóa hoặc không có sẵn." });
        }

        foreach (var seat in seatsToLock)
        {
            seat.LockedUntil = DateTime.UtcNow.Add(LockDuration); // Đặt thời gian khóa (5 phút từ bây giờ)
            seat.IsAvailable = false; // Đánh dấu là không có sẵn trong thời gian khóa
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

        return Ok(new { Message = "Khóa ghế thành công.", LockedSeats = lockedSeatDTOs });
    }

    // Mở khóa các ghế đã chọn
    [HttpPost("unlock")]
    public async Task<IActionResult> UnlockSeats([FromBody] List<int> seatIds)
    {
        if (seatIds == null || !seatIds.Any())
        {
            return BadRequest(new { Message = "Danh sách SeatIds là bắt buộc." });
        }

        var seats = await _context.Seats
            .Where(s => seatIds.Contains(s.Id))
            .ToListAsync();

        if (!seats.Any())
        {
            return NotFound(new { Message = "Không tìm thấy ghế nào trong danh sách yêu cầu để mở khóa. Kiểm tra lại SeatIds." });
        }

        foreach (var seat in seats)
        {
            seat.LockedUntil = null;
            seat.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Mở khóa ghế thành công.", UnlockedSeats = seatIds });
    }

    // Kiểm tra tính khả dụng của ghế (nếu chúng không bị khóa hoặc đã đặt)
    [HttpPost("CheckAvailability")]
    public IActionResult CheckAvailability([FromBody] List<int> seatIds)
    {
        if (seatIds == null || !seatIds.Any())
        {
            return BadRequest(new { Message = "Danh sách ghế không hợp lệ." });
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

    // Kiểm tra xem ghế có sẵn cho suất chiếu cụ thể hay không (cho vé đã đặt)
    [HttpPost("CheckSeats")]
    public async Task<IActionResult> CheckSeats([FromBody] SeatCheckRequest seatCheckDTO)
    {
        if (seatCheckDTO == null || seatCheckDTO.SeatIds == null || seatCheckDTO.SeatIds.Count == 0)
        {
            return BadRequest(new { Message = "Dữ liệu ghế không hợp lệ." });
        }

        var unavailableSeats = await _context.Tickets
            .Where(t => t.ShowtimeId == seatCheckDTO.ShowtimeId && seatCheckDTO.SeatIds.Contains(t.SeatId))
            .Select(t => t.SeatId)
            .ToListAsync();

        return Ok(new { UnavailableSeats = unavailableSeats });
    }

    // Khóa các ghế đã chọn khi chuyển sang trang đặt hàng
    [HttpPost("lock-and-continue")]
    public async Task<IActionResult> LockAndContinue([FromBody] LockSeatsRequestDTO request)
    {
        if (request.SeatIds == null || !request.SeatIds.Any())
        {
            return BadRequest(new { Message = "Danh sách SeatIds là bắt buộc." });
        }

        var seatsToLock = await _context.Seats
            .Where(s => request.SeatIds.Contains(s.Id) && s.IsAvailable && (!s.LockedUntil.HasValue || s.LockedUntil <= DateTime.UtcNow))
            .ToListAsync();

        if (!seatsToLock.Any())
        {
            return BadRequest(new { Message = "Không có ghế nào có sẵn để khóa." });
        }

        foreach (var seat in seatsToLock)
        {
            seat.LockedUntil = DateTime.UtcNow.Add(LockDuration); // Khóa ghế trong một khoảng thời gian nhất định
            seat.IsAvailable = false; // Cập nhật trạng thái khả dụng thành false (đã khóa)
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

        return Ok(new { Message = "Khóa ghế và tiếp tục.", LockedSeats = lockedSeatDTOs });
    }

    // Lấy danh sách ghế theo ID đơn hàng
    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetSeatsByOrderId(int orderId)
    {
        var seats = await _context.Seats
            .Where(s => s.OrderId == orderId)
            .ToListAsync();

        if (!seats.Any())
        {
            return NotFound(new { Message = $"No seats found for order ID {orderId}." });
        }

        var seatDTOs = seats.Select(seat => new SeatDTO
        {
            SeatId = seat.Id,
            Row = seat.Row,
            Number = seat.Number,
            IsAvailable = seat.IsAvailable,
            IsLocked = seat.LockedUntil.HasValue && seat.LockedUntil > DateTime.UtcNow,
            LockedUntil = seat.LockedUntil,
            ShowTimeId = seat.ShowTimeId,
            OrderId = seat.OrderId
        }).ToList();

        return Ok(seatDTOs);
    }

    // Tự động mở khóa ghế sau 5 phút
    [HttpPost("auto-unlock")]
    public async Task<IActionResult> AutoUnlockSeats()
    {
        var seatsToUnlock = await _context.Seats
            .Where(s => s.LockedUntil.HasValue && s.LockedUntil <= DateTime.UtcNow)
            .ToListAsync();

        if (!seatsToUnlock.Any())
        {
            return Ok(new { Message = "Không có ghế nào để mở khóa." });
        }

        foreach (var seat in seatsToUnlock)
        {
            seat.IsAvailable = true;
            seat.LockedUntil = null;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Mở khóa ghế thành công.", UnlockedSeats = seatsToUnlock.Select(s => s.Id).ToList() });
    }
}