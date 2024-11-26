using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateOrderAndTickets")]
        public async Task<IActionResult> CreateOrderAndTicketInfo([FromBody] OrderCreateDTO orderDTO)
        {
            if (orderDTO == null || orderDTO.UserId <= 0)
            {
                return BadRequest(new { Message = "Invalid order data or missing UserId." });
            }

            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.UserId == orderDTO.UserId);
                if (!userExists)
                {
                    return NotFound(new { Message = $"User with ID {orderDTO.UserId} not found." });
                }

                var newOrder = await CreateOrderAsync(orderDTO);

                if (orderDTO.Tickets != null && orderDTO.Tickets.Any())
                {
                    await CreateTicketsAndTicketInfoAsync(orderDTO.Tickets, newOrder.Id, orderDTO.UserId);
                }

                return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.Id }, new { newOrder.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the order and tickets.", Details = ex.Message });
            }
        }

        private async Task<Order> CreateOrderAsync(OrderCreateDTO orderDTO)
        {
            var newOrder = new Order
            {
                OrderDate = DateTime.UtcNow,
                ItemOrders = orderDTO.ItemOrders?.Select(io => new ItemOrder
                {
                    ItemId = io.ItemId,
                    Quantity = io.Quantity
                }).ToList() ?? new List<ItemOrder>()
            };

            decimal totalAmount = 0;
            foreach (var itemOrder in newOrder.ItemOrders)
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemId == itemOrder.ItemId);
                if (item != null)
                {
                    totalAmount += item.Price * itemOrder.Quantity;
                }
            }

            newOrder.TotalAmount = totalAmount;

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();
            return newOrder;
        }

        private async Task CreateTicketsAndTicketInfoAsync(IEnumerable<TicketCreateDTO> tickets, int orderId, int userId)
        {
            // Kiểm tra xem UserId có tồn tại trong bảng Users không
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                throw new Exception($"UserId {userId} does not exist.");
            }

            foreach (var ticketDTO in tickets)
            {
                var showtime = await _context.ShowTimes
                    .Include(s => s.Movie)
                    .Include(s => s.Theater)
                    .FirstOrDefaultAsync(s => s.ShowtimeId == ticketDTO.ShowtimeId);

                if (showtime == null || showtime.Movie == null || showtime.Theater == null)
                {
                    throw new Exception($"Invalid showtimeId {ticketDTO.ShowtimeId}, movieId, or theaterId not found.");
                }

                var newTicket = new Ticket
                {
                    ShowtimeId = ticketDTO.ShowtimeId,
                    SeatId = ticketDTO.SeatId,
                    Price = ticketDTO.Price,
                    UserId = userId,
                    MovieId = showtime.Movie.MovieId,
                    TheaterId = showtime.Theater.TheaterId
                };

                _context.Tickets.Add(newTicket);
                await _context.SaveChangesAsync();

                // Tạo TicketInfo
                var newTicketInfo = new TicketInfo
                {
                    TicketId = newTicket.TicketId,
                    OrderId = orderId,
                    UserId = userId
                };

                _context.TicketInfos.Add(newTicketInfo);

                // Cập nhật trạng thái ghế
                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.Id == ticketDTO.SeatId);
                if (seat != null)
                {
                    seat.IsAvailable = false; // Đánh dấu ghế là đã đặt
                    _context.Seats.Update(seat);
                }
            }

            await _context.SaveChangesAsync();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ItemOrders)
                .ThenInclude(io => io.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { Message = "Order not found." });
            }

            var orderResponse = new OrderResponseDTO
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ItemOrders = order.ItemOrders.Select(io => new ItemOrderResponseDTO
                {
                    ItemId = io.ItemId,
                    Quantity = io.Quantity,
                    ItemName = io.Item.Name
                }).ToList()
            };

            return Ok(orderResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ItemOrders)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { Message = "Order not found." });
            }

            try
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the order.", Details = ex.Message });
            }
        }
    }
}
