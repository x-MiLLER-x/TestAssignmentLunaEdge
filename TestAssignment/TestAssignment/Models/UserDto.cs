using System.ComponentModel.DataAnnotations;

namespace TestAssignment.Models
{
    public class UserDto
    {
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
