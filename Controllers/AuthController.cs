using CryptoWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace CryptoWallet.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Password { get; set; } // Tu frontend wysyła surowe hasło, a backend je zahashuje
        }

        // Rejestracja
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest("Użytkownik już istnieje.");
            else {
                // Tworzymy nowego użytkownika bez PasswordHash
                Console.WriteLine("Tworzymy go");
            var user = new User
            {
                Username = request.Username,
                PasswordHash = string.Empty
            };

            // Haszujemy hasło
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            var token = GenerateJwtToken(user);

                return Ok(new { token });
            }
        }


        // Logowanie
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var passwordHasher = new PasswordHasher<User>();
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser == null || passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, user.PasswordHash) == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Nieprawidłowy login lub hasło.");
            }

            var token = GenerateJwtToken(existingUser);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
