using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) 
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; i++) 
        {
            var r = UnityEngine.Random.Range(i, count);
            (ts[i], ts[r]) = (ts[r], ts[i]);
        }
    }

    public static int Modulo(this int a, int b)
    {
        return (Mathf.Abs(a * b) + a) % b;
    } 
}
