using System;
using System.Collections;
using UnityEngine;

public static class Utility 
{
    public static IEnumerator ExecuteAfterTime(Action function, float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);
        function();
    }
}
