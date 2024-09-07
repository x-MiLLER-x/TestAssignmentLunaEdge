namespace TestAssignment.Models
{
    public class User
    {
        public Guid Id { get; set; }  // Unique identifier for the user

        public string Username { get; set; }  // The user's username, required field

        public string Email { get; set; }  // The user's email address, required field

        public string PasswordHash { get; set; }  // Hashed password for the user

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Date when the user account was created, defaults to current UTC time

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Date when the user account was last updated, defaults to current UTC time

        public List<UserTask> Tasks { get; set; } = new List<UserTask>();  // List of tasks associated with the user, initialized as an empty list
    }
}
