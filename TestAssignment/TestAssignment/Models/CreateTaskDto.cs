namespace TestAssignment.Models
{
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
