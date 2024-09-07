using TestAssignment.Models;
using TestAssignment.Repositories;

namespace TestAssignment.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<UserTask>> GetTasksAsync(Guid userId, Models.TaskStatus? status, DateTime? dueDate, TaskPriority? priority, int pageNumber, int pageSize)
        {
            var tasks = await _taskRepository.GetTasksAsync(userId, status, dueDate, priority, pageNumber, pageSize);
            return tasks.ToList(); // Converting IEnumerable to List
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid id)
        {
            return await _taskRepository.GetTaskByIdAsync(id);
        }

        public async Task AddTaskAsync(UserTask task)
        {
            await _taskRepository.AddTaskAsync(task);
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            await _taskRepository.UpdateTaskAsync(task);
        }

        public async Task DeleteTaskAsync(Guid taskId)
        {
            await _taskRepository.DeleteTaskAsync(taskId);
        }
    }
}
