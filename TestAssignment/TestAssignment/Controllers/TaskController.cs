using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TestAssignment.Data;
using TestAssignment.Models;
using TestAssignment.Services;

namespace TestAssignment.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {

        private readonly ILogger<TaskController> _logger;
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context, ILogger<TaskController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks(
            Models.TaskStatus? status = null,
            DateTime? dueDate = null,
            TaskPriority? priority = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // Fix: Use IQueryable without type casting
            var query = _context.Tasks.Where(t => t.UserId.ToString() == userId).AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueDate.HasValue)
            {
                var dueDateOnly = dueDate.Value.Date;  // compare only dates, ignoring time
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= dueDateOnly);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority);
            }

            // Apply ordering and pagination
            var tasks = await query
                .Include(t => t.User) // Load related user data
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(tasks);
        }
        [HttpPost]
        public async Task<ActionResult<UserTask>> CreateTask(UserTask task)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            // Assign userId to the task
            task.UserId = new Guid(userId);

            // Set creation and update time to be the same
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            // Add the task to the context
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Load the task with the User field populated
            var createdTask = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UserTask task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || task.UserId.ToString() != userId)
            {
                return Unauthorized();
            }

            // Retrieve the existing task from the database
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == new Guid(userId));

            if (existingTask == null)
            {
                return NotFound();
            }

            // Preserve the original CreatedAt value
            task.CreatedAt = existingTask.CreatedAt;

            // Update the UpdatedAt field
            task.UpdatedAt = DateTime.UtcNow;

            // Update only the properties that have been changed, while keeping CreatedAt intact
            _context.Entry(existingTask).CurrentValues.SetValues(task);

            // Ensure CreatedAt is not updated
            _context.Entry(existingTask).Property(t => t.CreatedAt).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var task = await _context.Tasks.FindAsync(id);

            if (task == null || task.UserId.ToString() != userId)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
