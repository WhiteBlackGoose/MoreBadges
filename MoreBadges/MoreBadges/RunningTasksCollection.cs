public sealed class RunningTasksCollection
{
    private HashSet<Task> tasks = new();
    private int editsSinceLastGC = 0;
    private readonly object mutex = new();

    public void Add(Task task)
    {
        if (task.IsCompleted)
            return;
        lock (mutex)
        {
            tasks.Add(task);
            editsSinceLastGC++;
        }
    }

    private void CollectGarbage()
    {
        var atLeastOneCompleted = false;
        foreach (var task in tasks)
        {
            if (task.IsCompleted)
            {
                atLeastOneCompleted = true;
                break;
            }
        }
        if (!atLeastOneCompleted)
            return;
        tasks = tasks.Where(task => !task.IsCompleted).ToHashSet();
    }

    public int CountRunning()
    {
        CollectGarbage();
        return tasks.Count;
    }
}