using Microsoft.EntityFrameworkCore;
using TestAssignment.Data;
using TestAssignment.Models;

namespace TestAssignment.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(Guid userId, Models.TaskStatus? status, DateTime? dueDate, TaskPriority? priority, int pageNumber, int pageSize)
        {
            var query = _context.Tasks.Where(t => t.UserId == userId).AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueDate.HasValue)
            {
                var dueDateOnly = dueDate.Value.Date;
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= dueDateOnly);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority);
            }

            return await query
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .Skip((pageNumber - 1) * pageSize)
                .ToListAsync();
        }

        public async Task AddTaskAsync(UserTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(UserTask task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
