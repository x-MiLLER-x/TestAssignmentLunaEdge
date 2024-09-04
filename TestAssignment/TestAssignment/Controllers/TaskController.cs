using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TestAssignment.Data;
using TestAssignment.Models;

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

            var query = _context.Tasks.Where(t => t.UserId.ToString() == userId);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status);
            }

            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate == dueDate);
            }

            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == priority);
            }

            var tasks = await query
                .OrderBy(t => t.DueDate)
                .ThenBy(t => t.Priority)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<UserTask>> CreateTask(CreateTaskDto taskDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var task = new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                UserId = new Guid(userId) // Присваиваем UserId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
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

            _context.Entry(task).State = EntityState.Modified;

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
