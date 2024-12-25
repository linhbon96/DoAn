using Microsoft.AspNetCore.Mvc;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using MovieBookingApp.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Movie Lấy danh sách phim
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> GetMovies()
        {
            var movies = await _context.Movies.Include(m => m.Showtimes).ToListAsync();
            var movieDTOs = movies.Select(m => new MovieDTO
            {
                MovieId = m.MovieId,
                Title = m.Title,
                Description = m.Description,
                Genre = m.Genre,
                Duration = m.Duration,
                ReleaseDate = m.ReleaseDate,
                ImageUrl = m.ImageUrl
            }).ToList();

            return Ok(movieDTOs);
        }

        // GET: api/Movie/5 Lấy chi tiết phim
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(int id)
        {
            var movie = await _context.Movies
                                      .Include(m => m.Showtimes)
                                      .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound("Phim không tồn tại.");
            }

            var movieDTO = new MovieDTO
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                Genre = movie.Genre,
                Duration = movie.Duration,
                ReleaseDate = movie.ReleaseDate,
                ImageUrl = movie.ImageUrl
            };

            return Ok(movieDTO);
        }

        // POST: api/Movie Thêm mới phim
        [HttpPost]
        public async Task<ActionResult<Movie>> CreateMovie([FromBody] MovieCreateDTO movieDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movie = new Movie
            {
                Title = movieDTO.Title,
                Description = movieDTO.Description,
                Genre = movieDTO.Genre,
                Duration = movieDTO.Duration,
                ReleaseDate = DateTime.SpecifyKind(movieDTO.ReleaseDate, DateTimeKind.Utc),
                ImageUrl = movieDTO.ImageUrl
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, movie);
        }

        // thêm thông tin phim trên movieId
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdateDTO movieDTO)
        {
            if (id != movieDTO.MovieId)
            {
                return BadRequest("ID không khớp.");
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound("Phim không tồn tại.");
            }

            // Cập nhật thông tin movies

            movie.Title = movieDTO.Title;
            movie.Description = movieDTO.Description;
            movie.Genre = movieDTO.Genre;
            movie.Duration = movieDTO.Duration;
            movie.ReleaseDate = movieDTO.ReleaseDate;
            movie.ImageUrl = movieDTO.ImageUrl;

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound("Không thể cập nhật vì phim không tồn tại.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Tìm kiếm dựa trên tên phim có hỗ trợ chuẩn hoá tiếng việt
        [HttpGet("Search")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var movies = await _context.Movies
                .Where(m => EF.Functions.ILike(m.Title, $"%{query}%"))
                .Select(m => new MovieDTO
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    Description = m.Description,
                    Genre = m.Genre,
                    Duration = m.Duration,
                    ReleaseDate = m.ReleaseDate,
                    ImageUrl = m.ImageUrl
                })
                .ToListAsync();

            return Ok(movies);
        }


        // Hàm chuẩn hóa chuỗi tiếng Việt loại bỏ dấu để hỗ trợ khả năng tìm kiếm
        private string RemoveVietnameseTones(string str)
        {
            str = str.Normalize(NormalizationForm.FormD);
            char[] chars = str
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC)
                .Replace("đ", "d").Replace("Đ", "D");
        }

        //Xoá thông tin phim dựa trên movieId
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound("Phim không tồn tại.");
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.MovieId == id);
        }
    }
}
