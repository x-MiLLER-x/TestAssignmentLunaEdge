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
    [Authorize]  // Requires authentication for all endpoints
    [ApiController]  // Marks this class as an API controller
    [Route("api/[controller]")]  // API route convention
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly TaskService _taskService;

        // Constructor for dependency injection (context and logger)
        public TaskController(ApplicationDbContext context, ILogger<TaskController> logger, TaskService taskService)
        {
            _context = context;
            _logger = logger;
            _taskService = taskService;
        }

        [HttpGet]  // GET request to retrieve tasks
        public async Task<ActionResult<IEnumerable<UserTask>>> GetTasks(
            Models.TaskStatus? status = null,
            DateTime? dueDate = null,
            TaskPriority? priority = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Retrieve user ID from the JWT token
            if (userId == null)
            {
                return Unauthorized();  // If no userId, return Unauthorized
            }

            // Fetch tasks for the current user
            var query = _context.Tasks.Where(t => t.UserId.ToString() == userId).AsQueryable();

            // Apply filters based on query parameters
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueDate.HasValue)
            {
                var dueDateOnly = dueDate.Value.Date;  // Compare only the date part
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date <= dueDateOnly);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority);
            }

            // Apply ordering and pagination
            var tasks = await query
                .Include(t => t.User)  // Include related User data
                .OrderBy(t => t.DueDate)  // Order tasks by due date
                .ThenBy(t => t.Priority)  // Then by priority
                .Skip((pageNumber - 1) * pageSize)  // Pagination: skip previous pages
                .Take(pageSize)  // Limit to page size
                .ToListAsync();

            return Ok(tasks);  // Return tasks as HTTP 200 OK
        }

        [HttpPost]  // POST request to create a new task
        public async Task<ActionResult<UserTask>> CreateTask(UserTask task)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Retrieve user ID from token
            if (userId == null)
            {
                return Unauthorized();  // Return Unauthorized if no user ID
            }

            // Assign the userId to the new task
            task.UserId = new Guid(userId);

            // Set creation and updated time to current time
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            // Add the task to the database
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Load the task again to include the User data
            var createdTask = await _context.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == task.Id);

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, createdTask);  // Return HTTP 201 Created
        }

        [HttpPut("{id}")]  // PUT request to update an existing task by ID
        public async Task<IActionResult> UpdateTask(Guid id, UserTask task)
        {
            // Check if the task ID matches the route ID
            if (id != task.Id)
            {
                return BadRequest();  // Return BadRequest if IDs don't match
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Retrieve user ID from token
            if (userId == null || task.UserId.ToString() != userId)
            {
                return Unauthorized();  // Return Unauthorized if user IDs don't match
            }

            // Retrieve the existing task from the database
            var existingTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == new Guid(userId));

            if (existingTask == null)
            {
                return NotFound();  // Return NotFound if the task doesn't exist
            }

            // Preserve the original CreatedAt value
            task.CreatedAt = existingTask.CreatedAt;

            // Update the UpdatedAt field to current time
            task.UpdatedAt = DateTime.UtcNow;

            // Update all the properties of the existing task except CreatedAt
            _context.Entry(existingTask).CurrentValues.SetValues(task);
            _context.Entry(existingTask).Property(t => t.CreatedAt).IsModified = false;  // Prevent updating CreatedAt

            try
            {
                await _context.SaveChangesAsync();  // Save changes to the database
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))  // Check if the task still exists
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();  // Return HTTP 204 No Content on success
        }

        [HttpDelete("{id}")]  // DELETE request to delete a task by ID
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // Retrieve user ID from token
            var task = await _context.Tasks.FindAsync(id);  // Find the task by ID

            // Check if the task exists and belongs to the user
            if (task == null || task.UserId.ToString() != userId)
            {
                return NotFound();  // Return NotFound if task doesn't exist or belongs to another user
            }

            _context.Tasks.Remove(task);  // Remove the task from the database
            await _context.SaveChangesAsync();  // Save changes to the database

            return NoContent();  // Return HTTP 204 No Content on success
        }

        // Helper method to check if a task exists by ID
        private bool TaskExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
