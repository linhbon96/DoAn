using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get tickets by UserId
[HttpGet("user/{userId}")]
public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTicketsByUser(int userId)
{
    var tickets = await _context.Tickets
        .Include(t => t.Seat)
        .Include(t => t.Showtime)
        .ThenInclude(st => st.Movie)
        .Where(t => t.UserId == userId)
        .ToListAsync();

    if (!tickets.Any())
    {
        return NotFound(new { Message = "No tickets found for this user." });
    }

    var ticketDTOs = tickets.Select(t => new TicketDTO
    {
        TicketId = t.TicketId,
        ShowtimeId = t.ShowtimeId,
        MovieTitle = t.Showtime.Movie.Title,
        ShowDate = t.Showtime.ShowDate,
        ShowHour = t.Showtime.ShowHour,
        SeatId = t.SeatId,
        Row = t.Seat.Row,
        Number = t.Seat.Number,
        Price = t.Price,
        MovieId = t.Showtime.MovieId,
        TheaterId = t.Showtime.TheaterId
    }).ToList();

    return Ok(ticketDTOs);
}

        // Create tickets
       [HttpPost]
public async Task<IActionResult> CreateTickets([FromBody] List<TicketCreateDTO> ticketDTOs)
{
    if (ticketDTOs == null || !ticketDTOs.Any())
    {
        return BadRequest(new { Message = "No ticket data provided." });
    }

    try
    {
        var newTickets = ticketDTOs.Select(dto => new Ticket
        {
            ShowtimeId = dto.ShowtimeId,
            SeatId = dto.SeatId,
            Price = dto.Price,
            MovieId = dto.MovieId, // Gửi từ frontend
            TheaterId = dto.TheaterId, // Gửi từ frontend
            UserId = dto.UserId // Gửi từ frontend nếu cần
        }).ToList();

        _context.Tickets.AddRange(newTickets);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Tickets created successfully." });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = "Error creating tickets.", Details = ex.InnerException?.Message ?? ex.Message });
    }
}



        // Get all tickets
        [HttpGet]
public async Task<ActionResult<IEnumerable<TicketDTO>>> GetAllTickets()
{
    var tickets = await _context.Tickets
        .Include(t => t.Seat)
        .Include(t => t.Showtime)
        .ThenInclude(st => st.Movie)
        .ToListAsync();

    var ticketDTOs = tickets.Select(t => new TicketDTO
    {
        TicketId = t.TicketId,
        ShowtimeId = t.ShowtimeId,
        MovieTitle = t.Showtime.Movie.Title,
        ShowDate = t.Showtime.ShowDate,
        ShowHour = t.Showtime.ShowHour,
        SeatId = t.SeatId,
        Row = t.Seat.Row,
        Number = t.Seat.Number,
        Price = t.Price,
        MovieId = t.Showtime.MovieId,
        TheaterId = t.Showtime.TheaterId
    }).ToList();

    return Ok(ticketDTOs);
}


        // Delete a ticket by ID
        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(new { Message = "Ticket not found." });
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Ticket deleted successfully." });
        }
    }
}
