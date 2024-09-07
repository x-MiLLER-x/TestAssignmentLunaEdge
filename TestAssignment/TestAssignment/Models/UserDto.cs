using System.ComponentModel.DataAnnotations;

namespace TestAssignment.Models
{
    public class UserDto
    {
        // The username provided by the user during registration
        public string Username { get; set; }

        // The user's email address, validated to ensure correct email format
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        // The password entered by the user, required for registration
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}