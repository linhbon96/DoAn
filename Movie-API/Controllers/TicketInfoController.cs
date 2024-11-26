using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get ticket infos by UserId
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTicketInfosByUserId(int userId)
        {
            var ticketInfos = await _context.TicketInfos
                .Include(ti => ti.Ticket)
                .Include(ti => ti.User)
                .Where(ti => ti.UserId == userId)
                .ToListAsync();

            if (!ticketInfos.Any())
                return NotFound(new { Message = "No ticket information found for the specified user." });

            var ticketInfoDTOs = ticketInfos.Select(ti => new TicketInfoDTO
            {
                TicketInfoId = ti.TicketInfoId,
                TicketId = ti.TicketId,
                OrderId = ti.OrderId,
                UserId = ti.UserId,
                UserName = ti.User?.Username,
                TicketDetails = ti.Ticket != null 
                    ? $"Ticket ID: {ti.Ticket.TicketId}" 
                    : null
            });

            return Ok(ticketInfoDTOs);
        }

        // Get ticket info by TicketInfoId
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetTicketInfoById(int id)
        {
            var ticketInfo = await _context.TicketInfos
                .Include(ti => ti.Ticket)
                .Include(ti => ti.User)
                .FirstOrDefaultAsync(ti => ti.TicketInfoId == id);

            if (ticketInfo == null)
                return NotFound(new { Message = "Ticket info not found." });

            var ticketInfoDTO = new TicketInfoDTO
            {
                TicketInfoId = ticketInfo.TicketInfoId,
                TicketId = ticketInfo.TicketId,
                OrderId = ticketInfo.OrderId,
                UserId = ticketInfo.UserId,
                UserName = ticketInfo.User?.Username,
                TicketDetails = ticketInfo.Ticket != null 
                    ? $"Ticket ID: {ticketInfo.Ticket.TicketId}" 
                    : null
            };

            return Ok(ticketInfoDTO);
        }

        // Create ticket info
        [HttpPost]
        public async Task<IActionResult> CreateTicketInfo([FromBody] TicketInfoCreateDTO ticketInfoCreateDTO)
        {
            // Kiểm tra nếu Ticket hoặc Order không tồn tại
            var ticketExists = await _context.Tickets.AnyAsync(t => t.TicketId == ticketInfoCreateDTO.TicketId);
            if (!ticketExists)
                return BadRequest(new { Message = "Invalid TicketId." });

            if (ticketInfoCreateDTO.OrderId.HasValue)
            {
                var orderExists = await _context.Orders.AnyAsync(o => o.Id == ticketInfoCreateDTO.OrderId.Value);
                if (!orderExists)
                    return BadRequest(new { Message = "Invalid OrderId." });
            }

            var ticketInfo = new TicketInfo
            {
                TicketId = ticketInfoCreateDTO.TicketId,
                OrderId = ticketInfoCreateDTO.OrderId,
                UserId = ticketInfoCreateDTO.UserId
            };

            _context.TicketInfos.Add(ticketInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicketInfoById), new { id = ticketInfo.TicketInfoId }, ticketInfo);
        }

        // Delete ticket info
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketInfo(int id)
        {
            var ticketInfo = await _context.TicketInfos.FindAsync(id);
            if (ticketInfo == null)
                return NotFound(new { Message = "Ticket info not found." });

            _context.TicketInfos.Remove(ticketInfo);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Ticket info deleted successfully." });
        }
    }
}
