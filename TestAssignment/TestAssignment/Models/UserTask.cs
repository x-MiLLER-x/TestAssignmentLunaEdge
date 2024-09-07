using System.Text.Json.Serialization;

namespace TestAssignment.Models
{
    public class UserTask
    {
        public Guid Id { get; set; }  // Unique identifier for each task

        public string Title { get; set; }  // Title of the task, required field

        public string? Description { get; set; }  // Optional description of the task

        public DateTime? DueDate { get; set; }  // Optional due date for the task

        public TaskStatus Status { get; set; }  // Status of the task (Pending, InProgress, Completed)

        public TaskPriority Priority { get; set; }  // Priority level of the task (Low, Medium, High)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Automatically set creation date (default to current UTC time)

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // Automatically set update date (default to current UTC time)

        public Guid? UserId { get; set; }  // Identifier for the user who owns the task, optional for now

        [JsonIgnore]  // Prevents the User property from being serialized in the JSON response
        public User? User { get; set; }  // Navigation property to the User model, optional
    }

    // Enumeration representing the status of a task
    public enum TaskStatus
    {
        Pending,  // Task is not started
        InProgress,  // Task is currently in progress
        Completed  // Task is completed
    }

    // Enumeration representing the priority of a task
    public enum TaskPriority
    {
        Low,  // Low priority task
        Medium,  // Medium priority task
        High  // High priority task
    }
}
