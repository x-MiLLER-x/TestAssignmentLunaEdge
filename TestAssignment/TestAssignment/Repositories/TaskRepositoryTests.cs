using Microsoft.EntityFrameworkCore;
using TestAssignment.Data;
using TestAssignment.Models;
using TestAssignment.Repositories;
using Xunit;

public class TaskRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly TaskRepository _taskRepository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _taskRepository = new TaskRepository(_context);

        // Seed test data
        _context.Tasks.Add(new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var task = _context.Tasks.First();

        // Act
        var result = await _taskRepository.GetTaskByIdAsync(task.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldAddNewTask()
    {
        // Arrange
        var newTask = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "New Task",
            Description = "New Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        await _taskRepository.AddTaskAsync(newTask);

        // Assert
        var addedTask = _context.Tasks.FirstOrDefault(t => t.Id == newTask.Id);
        Assert.NotNull(addedTask);
        Assert.Equal("New Task", addedTask.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTask()
    {
        // Arrange: create a new task
        var newTask = new UserTask
        {
            Id = Guid.NewGuid(),
            Title = "Original Task",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add the new task to the context
        await _taskRepository.AddTaskAsync(newTask);

        // Retrieve the task from the database
        var taskInDb = await _taskRepository.GetTaskByIdAsync(newTask.Id);
        Assert.NotNull(taskInDb);  // Ensure that the task was added

        // Save the current UpdatedAt value
        var previousUpdatedAt = taskInDb.UpdatedAt;

        // Delay to ensure time difference for the update
        await Task.Delay(1000); // 1 second

        // Update the task properties
        taskInDb.Title = "Updated Task";
        taskInDb.Description = "Updated Description";
        taskInDb.UpdatedAt = DateTime.UtcNow; // Explicitly update the time

        // Act: update the task
        await _taskRepository.UpdateTaskAsync(taskInDb);

        // Reload the task from the database
        var updatedTaskInDb = await _taskRepository.GetTaskByIdAsync(newTask.Id);

        // Assert: verify that the update occurred
        Assert.NotNull(updatedTaskInDb);
        Assert.Equal("Updated Task", updatedTaskInDb.Title);
        Assert.Equal("Updated Description", updatedTaskInDb.Description);
        Assert.Equal(newTask.CreatedAt, updatedTaskInDb.CreatedAt); // CreatedAt should remain the same
        Assert.True(updatedTaskInDb.UpdatedAt > previousUpdatedAt); // Ensure that UpdatedAt has changed
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
    {
        // Arrange
        var task = _context.Tasks.First();

        // Act
        await _taskRepository.DeleteTaskAsync(task.Id);

        // Assert
        var deletedTask = _context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        Assert.Null(deletedTask);
    }
}
