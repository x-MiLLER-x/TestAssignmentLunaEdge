using TestAssignment.Models;

namespace TestAssignment.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<UserTask>> GetTasksAsync(Guid userId, Models.TaskStatus? status, DateTime? dueDate, TaskPriority? priority, int pageNumber, int pageSize);
        Task<UserTask> GetTaskByIdAsync(Guid id);
        Task AddTaskAsync(UserTask task);
        Task UpdateTaskAsync(UserTask task);
        Task DeleteTaskAsync(Guid id);
    }
}
