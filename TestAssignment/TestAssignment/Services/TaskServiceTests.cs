using Moq;
using TestAssignment.Models;
using TestAssignment.Repositories;
using TestAssignment.Services;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _taskService = new TaskService(_taskRepositoryMock.Object);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new UserTask { Id = taskId, Title = "Test Task" };
        _taskRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _taskService.GetTaskByIdAsync(taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
    }

    [Fact]
    public async Task AddTaskAsync_ShouldInvokeRepositoryAddMethod()
    {
        // Arrange
        var newTask = new UserTask { Id = Guid.NewGuid(), Title = "New Task" };

        // Act
        await _taskService.AddTaskAsync(newTask);

        // Assert
        _taskRepositoryMock.Verify(repo => repo.AddTaskAsync(newTask), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldInvokeRepositoryUpdateMethod()
    {
        // Arrange
        var taskToUpdate = new UserTask { Id = Guid.NewGuid(), Title = "Update Task" };

        // Act
        await _taskService.UpdateTaskAsync(taskToUpdate);

        // Assert
        _taskRepositoryMock.Verify(repo => repo.UpdateTaskAsync(taskToUpdate), Times.Once);
    }

    // Add more tests for other service methods
}
