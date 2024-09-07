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

    // Add more tests for UpdateTaskAsync and DeleteTaskAsync
}
