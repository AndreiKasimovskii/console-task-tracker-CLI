using System.Text.Json;
using TaskTracker.Infrastructure.Data;

namespace TaskTracker.Infrastructure;

public class FileStore
{
    private readonly string _filePath = Path.Combine(Environment.CurrentDirectory, "tasks.json");

    public async ValueTask<StorageModel> ReadFromFileAsync()
    {
        if(!File.Exists(_filePath))
            return new StorageModel();

        await using var readedStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        var result = (await JsonSerializer.DeserializeAsync<StorageModel>(readedStream)) ?? new();

        return result;
    }

    public async Task WriteToFileAsync(StorageModel storageModel)
    {
        var tempFilePath = Path.Combine(Environment.CurrentDirectory, Path.GetTempFileName());
        await using var writedStream = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.Write);
        await JsonSerializer.SerializeAsync(writedStream, storageModel);
        File.Move(tempFilePath, _filePath, true);
    }
}
