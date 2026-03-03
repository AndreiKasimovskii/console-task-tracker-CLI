using System.Collections.ObjectModel;
using Moq;
using TaskTracker.Domain.Abstractions;
using TaskTracker.Domain.Data;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Services;

namespace TaskTracker.Domain.Tests;

public class UserTaskServiceTest
{
  [Fact]
  public async Task ShowTaskListTest()
  {
    #region Arrange
    UserTask[] activeTasks = [.. tasks.Where(t => t.Status == UserTaskStatus.Active)];
    int exptectedTasksCount = activeTasks.Length;
    var userTaskRepositoryMock = new Mock<IUserTaskRepository>();
    userTaskRepositoryMock.Setup(r => r.GetAllActive())
      .ReturnsAsync(activeTasks);

    var service = new UserTaskService(userTaskRepositoryMock.Object);
    #endregion

    #region Act
    var actual = await service.ShowTasksList();
    #endregion

    #region Assert
    Assert.Equal(exptectedTasksCount, actual.Count);
    Assert.Equivalent(activeTasks, actual);
    #endregion
  }

  [Fact]
  public async Task ShowTaskTest_Failure()
  {
    #region Arrange
    var taskId = 100;
    var exptectedResult = OperationResult.Failure(ErrorType.NotFound);
    var userTaskRepositoryMock = new Mock<IUserTaskRepository>();
    userTaskRepositoryMock.Setup(r => r.GetById(It.IsAny<long>()))
      .Returns(Task.FromResult((UserTask?)null));

    var service = new UserTaskService(userTaskRepositoryMock.Object);
    #endregion

    #region Act
    var actual = await service.ShowTask(taskId);
    #endregion

    #region Assert
    Assert.Equal(exptectedResult, actual);
    #endregion
  }

  private readonly IEnumerable<UserTask> tasks =
      from n in Enumerable.Range(1, 20)
      select UserTask.Restore(n, $"Title {n}", $"Description {n}",
          DateTimeOffset.Now.AddDays(-n), ((UserTaskStatus)(n % Enum.GetValues<UserTaskStatus>().Length)).ToString(),
          n % 2 == 0 ? DateTimeOffset.Now.AddDays(n) : null);
}
