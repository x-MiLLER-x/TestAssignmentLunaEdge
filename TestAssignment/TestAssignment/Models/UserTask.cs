using System.Text.Json.Serialization;

namespace TestAssignment.Models
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; } // Task title is still required
        public string? Description { get; set; } // Task description can be optional
        public DateTime? DueDate { get; set; } // Due date can be optional
        public TaskStatus Status { get; set; } // Task status is required
        public TaskPriority Priority { get; set; } // Task priority is required
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation date is required
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UserId { get; set; } // UserId can be nullable if it's optional

        [JsonIgnore]  // Avoid cyclic dependencies
        public User? User { get; set; } // User field can also be nullable
    }

    public enum TaskStatus { Pending, InProgress, Completed }
    public enum TaskPriority { Low, Medium, High }
}
