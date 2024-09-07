namespace TestAssignment.Models
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; }  // The user can log in using either their username or email

        public string Password { get; set; }  // The plain text password provided by the user for authentication
    }
}
