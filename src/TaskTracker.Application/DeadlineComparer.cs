namespace TaskTracker.Application;

class DeadlineComparer : IComparer<DateTimeOffset?>
{
    public int Compare(DateTimeOffset? x, DateTimeOffset? y)
    {
      if (x is null && y is null)
        return 0;

      if (x is null && y is not null)
        return 1;

      if (x is not null && y is null)
        return -1;

      return ((DateTimeOffset)x!).CompareTo((DateTimeOffset)y!);
    }
}