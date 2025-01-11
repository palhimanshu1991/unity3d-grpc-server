using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> _taskQueue = new ConcurrentQueue<Action>();

    public static Task<T> ScheduleAsync<T>(Func<T> task)
    {
        var tcs = new TaskCompletionSource<T>();
        _taskQueue.Enqueue(() =>
        {
            try
            {
                // Execute the task and set the result
                var result = task();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                tcs.SetException(ex);
            }
        });
        return tcs.Task; // Return the task to the caller
    }

    public static Task ScheduleAsync(Action task)
    {
        var tcs = new TaskCompletionSource<object>();
        _taskQueue.Enqueue(() =>
        {
            try
            {
                // Execute the task
                task();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                tcs.SetException(ex);
            }
        });
        return tcs.Task; // Return the task to the caller
    }

    private void Update()
    {
        while (_taskQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
