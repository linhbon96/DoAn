using Microsoft.AspNetCore.Mvc;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả các món ăn/đồ uống
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItems()
        {
            return await _context.Items
                .Select(i => new ItemDTO
                {
                    ItemId = i.ItemId,
                    Name = i.Name,
                    Price = i.Price
                })
                .ToListAsync();
        }

        // Lấy thông tin món ăn/đồ uống theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return new ItemDTO
            {
                ItemId = item.ItemId,
                Name = item.Name,
                Price = item.Price
            };
        }

        // Thêm món ăn/đồ uống mới
        [HttpPost]
        public async Task<ActionResult<ItemDTO>> CreateItem(ItemDTO itemDto)
        {
            var item = new Item
            {
                Name = itemDto.Name,
                Price = itemDto.Price
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.ItemId }, itemDto);
        }

        // Cập nhật món ăn/đồ uống
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemDTO itemDto)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            item.Name = itemDto.Name;
            item.Price = itemDto.Price;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Xóa món ăn/đồ uống
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
