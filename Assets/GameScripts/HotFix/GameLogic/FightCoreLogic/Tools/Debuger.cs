using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuger
{
    public static void Log(object message)
    {
#if CLIENT_LOGIC
        Debug.Log(message);
#else
        Console.WriteLine(message);
#endif
    }
    public static void Log(object message,int index)
    {
#if CLIENT_LOGIC
        Debug.Log($"<color=#FF003F>{message}</color>");
#else
        Console.WriteLine(message);
#endif
    }
    public static void LogError(object message)
    {
#if CLIENT_LOGIC
        Debug.Log(message);
#else
        Console.WriteLine(message);
#endif
    }
}
