using TaskTracker.Domain.Entities;

namespace TaskTracker.Domain.Abstractions;

public interface IUserTaskRepository
{
    Task<UserTask?> GetById(long id);

    Task<UserTask> Create(UserTask task);

    Task<UserTask> Update(UserTask task);

    Task Delete(long id);

    /// <summary>
    /// Получить все задачи в статусе "Active" из источника
    /// </summary>
    /// <exception cref="TimeoutException">
    /// Генерируется, когда ожидание доступа к ресурсу превысило 30 секунд
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// Генерируется, когда состояние хранилища повреждено
    /// </exception>
    /// <returns>Список задач в статусе "Active"</returns>
    Task<UserTask[]> GetAllActive();

    Task<bool> HasDuplicate(string title, DateTimeOffset? deadline, long excludeId = -1);
}
