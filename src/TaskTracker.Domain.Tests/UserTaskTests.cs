using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Domain.Tests;

public class UserTaskTests
{
  private readonly IEnumerable<UserTask> _testData =
    from n in Enumerable.Range(1, 10)
    select UserTask.Restore(
      id: n,
      title: $"Title{n}",
      description: "",
      createdDate: n % 2 == 0
        ? new DateTimeOffset(new DateTime(2026, 02, 05))
        : new DateTimeOffset(new DateTime(2026, 02, 10)),
      status: ((UserTaskStatus)(n % Enum.GetValues<UserTaskStatus>().Length)).ToString(),
      deadline: null);
  
  public static TheoryData<long, string, DateTimeOffset, string, DateTimeOffset?, string> RestoreFailureCases =
    new()
    {
      { -1, "Title 1", new DateTimeOffset(new DateTime(2026, 01, 20)), "Active", null, "Error! Id lower then 0." },
      { 2, "", new DateTimeOffset(new DateTime(2026, 01, 20)), "Active", null, "Error! Title is empty." },
      { 3, "Title 3", default, "Active", null, "Error! Creation date not set." },
      {
        4, "Title 4", new DateTimeOffset(new DateTime(2026, 01, 20)), "", null, "Error! Task status has unknown value."
      },
      {
        5, "Title 5", new DateTimeOffset(new DateTime(2026, 01, 20)), "Active",
        new DateTimeOffset(new DateTime(2025, 12, 20)),
        "Error! Task deadline cannot be earlier than the creation date"
      },
    };
  
  [Theory, MemberData(nameof(RestoreFailureCases))]
  public void RestoreTest_Failure_StorageCorrupted(
    long id, 
    string title, 
    DateTimeOffset createdDate,
    string status,
    DateTimeOffset? deadline,
    string exceptionMessage
    )
  {
    #region Act/Assert

    var exception = Assert.Throws<StorageCorruptedException>(() => UserTask.Restore(id, title, "", createdDate, status, deadline));
    Assert.Equal(exceptionMessage, exception.Message);

    #endregion
  }

  [Fact]
  public void RestoreTest_Success()
  {
    #region Arrange

    long id = 1;
    var title = "Test title";
    var createdDate = new DateTimeOffset(new DateTime(2026, 01, 20));
    var status = "Active";
    DateTimeOffset? deadline = null;
    var expectedStatus = Enum.Parse<UserTaskStatus>(status);
    #endregion

    #region Act

    var userTask = UserTask.Restore(id, title, "", createdDate, status, deadline);

    #endregion

    #region Assert
    Assert.Equal(id, userTask.Id);
    Assert.Equal(title, userTask.Title);
    Assert.Equal(createdDate, userTask.CreatedDate);
    Assert.Equal(expectedStatus, userTask.Status);
    Assert.Equal(deadline, userTask.Deadline);
    #endregion
  }

  [Theory]
  [InlineData(UserTaskStatus.Active, 3)]
  [InlineData(UserTaskStatus.Cancelled, 1)]
  [InlineData(UserTaskStatus.Completed, 1)]
  public void TryChangeStatusTest_Failure(UserTaskStatus newStatus, long taskId)
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == taskId);

    #endregion

    #region Act/Assert

    Assert.False(userTask.TryChangeStatus(newStatus));

    #endregion
  }

  [Fact]
  public void TryChangeStatusTest_Success()
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == 3);

    #endregion

    #region Act/Assert

    Assert.True(userTask.TryChangeStatus(UserTaskStatus.Completed));

    #endregion
  }

  [Fact]
  public void TryChangeTitleTest_Failure()
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == 1);

    #endregion

    #region Act/Assert

    Assert.False(userTask.TryChangeTitle(null));

    #endregion
  }

  [Fact]
  public void TryChangeTitleTest_Success()
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == 1);
    var newTitle = "New title";
    #endregion

    #region Act/Assert

    Assert.True(userTask.TryChangeTitle(newTitle));
    Assert.Equal(newTitle, userTask.Title);
    #endregion
  }

  [Fact]
  public void TrySetDeadlineTest_Failure()
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == 1);

    #endregion

    #region Act/Assert

    Assert.False(userTask.TrySetDeadline(new DateTimeOffset(new DateTime(2026, 2, 1))));

    #endregion
  }

  [Fact]
  public void TrySetDeadlineTest_Success()
  {
    #region Arrange

    var userTask = _testData.First(t => t.Id == 1);
    var newDeadline = new DateTimeOffset(new DateTime(2026, 2, 28));
    #endregion

    #region Act/Assert

    Assert.True(userTask.TrySetDeadline(newDeadline));
    Assert.Equal(newDeadline, userTask.Deadline);
    #endregion
  }
}
