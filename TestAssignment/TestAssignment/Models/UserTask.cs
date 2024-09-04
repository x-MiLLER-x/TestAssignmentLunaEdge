using System.Text.Json.Serialization;

namespace TestAssignment.Models
{
    public class UserTask
    {
        public Guid Id { get; set; }
        public string Title { get; set; } // Название задачи все еще обязательно
        public string? Description { get; set; } // Описание задачи может быть необязательным
        public DateTime? DueDate { get; set; } // Дата завершения может быть необязательной
        public TaskStatus Status { get; set; } // Статус задачи обязательный
        public TaskPriority Priority { get; set; } // Приоритет задачи обязательный
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания обязательная
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Дата обновления обязательная
        public Guid? UserId { get; set; } // Поле UserId может быть nullable, если оно необязательно

        [JsonIgnore]  // Избегаем циклической зависимости
        public User? User { get; set; } // Поле User также может быть nullable
    }

    public enum TaskStatus { Pending, InProgress, Completed }
    public enum TaskPriority { Low, Medium, High }
}
