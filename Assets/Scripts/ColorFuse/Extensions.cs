
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


#region  Extensions
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    public static void Shuffle<T>(this IList<T> list, System.Random rng)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static T PickRandom<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new System.InvalidOperationException("Liste boş!");

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }

}

public static class TaskExtensions
{
    public static void Forget(this Task task)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted) Debug.LogException(t.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}

#endregion

