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

        // Constructor to inject dependencies
        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST /login - Endpoint to authenticate a user and generate a JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            // Validate if username or email is provided
            if (string.IsNullOrEmpty(request.UsernameOrEmail))
            {
                return BadRequest(new { message = "Username or Email is required." });
            }

            User user = null;

            // Check if the input is an email or username
            if (request.UsernameOrEmail.Contains("@"))
            {
                // If input is an email, find user by email
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail);
            }
            else
            {
                // If input is a username, find user by username
                user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail);
            }

            // Check if user exists and the password is correct
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Generate a JWT token for the authenticated user
            var token = CreateJwtToken(user);
            return Ok(new { token });
        }

        // Helper method to create a JWT token for the authenticated user
        private string CreateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Create a symmetric security key and signing credentials
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // Define the claims to be included in the JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // Subject claim (User ID)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Unique ID for the token
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // User ID as NameIdentifier
                new Claim(ClaimTypes.Name, user.Username)  // Username claim
            };

            // Create the JWT token with the claims and signing credentials
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),  // Token expiration time
                signingCredentials: creds);

            // Return the token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST /register - Endpoint to register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            // Validate the user's password using the ValidatePassword method
            var passwordValidationResult = ValidatePassword(request.Password);
            if (!passwordValidationResult.IsValid)
            {
                return BadRequest(new { message = passwordValidationResult.ErrorMessage });
            }

            // Create a new user object
            var user = new User
            {
                Id = Guid.NewGuid(),  // Generate a new unique ID for the user
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)  // Hash the password for security
            };

            // Add the new user to the database and save changes
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // Helper method to validate the user's password based on length and special characters
        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            int minLength = 8;  // Minimum length for the password
            bool requireSpecialChar = true;  // Flag to check if a special character is required

            // Check if the password meets the minimum length requirement
            if (password.Length < minLength)
            {
                return (false, $"Password must be at least {minLength} characters long.");
            }

            // Check if the password contains at least one special character
            if (requireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return (false, "Password must contain at least one special character.");
            }

            // If the password is valid, return true
            return (true, null);
        }
    }
}