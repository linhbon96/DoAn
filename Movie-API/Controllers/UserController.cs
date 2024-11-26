using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MovieBookingApp.Data;
using MovieBookingApp.Models;
using MovieBookingApp.Models.DTOs;

namespace MovieBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userRegisterDTO)
        {
            if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
            {
                return BadRequest("Password and Confirm Password do not match.");
            }

            if (await _context.Users.AnyAsync(u => u.Username == userRegisterDTO.Username))
            {
                return Conflict("Username already exists.");
            }

            var user = new User
            {
                Username = userRegisterDTO.Username,
                PasswordHash = ComputeMD5Hash(userRegisterDTO.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            if (string.IsNullOrEmpty(userLoginDTO.Username) || string.IsNullOrEmpty(userLoginDTO.Password))
            {
                return BadRequest("Username or password cannot be empty.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userLoginDTO.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username.");
            }

            if (user.PasswordHash != ComputeMD5Hash(userLoginDTO.Password))
            {
                return Unauthorized("Invalid password.");
            }

            return Ok(new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            });
        }

        private static string ComputeMD5Hash(string input)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return string.Concat(hashBytes.Select(b => b.ToString("X2")));
        }
    }
}
