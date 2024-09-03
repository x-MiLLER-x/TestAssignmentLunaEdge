using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TestAssignment.Data;
using TestAssignment.Models;
using Microsoft.EntityFrameworkCore;

namespace TestAssignment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            if (string.IsNullOrEmpty(request.UsernameOrEmail))
            {
                return BadRequest(new { message = "Username or Email is required." });
            }

            User user = null;

            if (request.UsernameOrEmail.Contains("@"))
            {
                // Если введен email
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail);
            }
            else
            {
                // Если введено имя пользователя
                user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail);
            }

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = CreateJwtToken(user);
            return Ok(new { token });
        }


        private string CreateJwtToken(User user)
        {
            // Получение ключа, издателя и аудитории из конфигурации
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), "JWT Key cannot be null or empty.");
            }

            var issuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException(nameof(issuer), "JWT Issuer cannot be null or empty.");
            }

            var audience = _configuration["Jwt:Audience"];
            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentNullException(nameof(audience), "JWT Audience cannot be null or empty.");
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            var passwordValidationResult = ValidatePassword(request.Password);
            if (!passwordValidationResult.IsValid)
            {
                return BadRequest(new { message = passwordValidationResult.ErrorMessage });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            int minLength = 8;
            bool requireSpecialChar = true;

            if (password.Length < minLength)
            {
                return (false, $"Password must be at least {minLength} characters long.");
            }

            if (requireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return (false, "Password must contain at least one special character.");
            }

            return (true, null);
        }
    }
}
