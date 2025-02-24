﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public static class Debuger
{
    public static void Log(object message)
    {
#if CLIENT_LOGIC
        Debug.Log(message);
#else
        Console.WriteLine(message);
#endif
    }

    public static void LogError(object message)
    {
#if CLIENT_LOGIC
        Debug.LogError(message);
#else
        Console.WriteLine(message);
#endif

    }
}
