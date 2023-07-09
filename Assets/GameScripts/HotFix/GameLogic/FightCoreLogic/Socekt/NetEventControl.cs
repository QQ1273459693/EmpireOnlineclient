using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 事件处理中心，负责事件的注册和事件的派发
/// </summary>
public class NetEventControl
{
    /// <summary>
    /// 互斥
    /// </summary>
    private static object nMutex = new object();
    /// <summary>
    /// 委托事件
    /// </summary>
    public delegate void EventHandler(byte[] bytes);
    /// <summary>
    /// 事件派发注册字典
    /// </summary>
    private static Dictionary<Protocal, List<EventHandler>> eventCtrlDic = new Dictionary<Protocal, List<EventHandler>>();
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="eventType">事件类型.</param>
    /// <param name="handler">事件函数.</param>
    public static void AddEvent(Protocal eventType,EventHandler handler)
    {
        lock (nMutex)
        {
            //如果该事件没有注册 创建事件
            if (!eventCtrlDic.ContainsKey(eventType))
            {
                eventCtrlDic.Add(eventType, new List<EventHandler>());
            }
            //如果该事件没有注册函数
            if (!eventCtrlDic[eventType].Contains(handler))
            {
                eventCtrlDic[eventType].Add(handler);
            }
        }
    }
    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="eventType">事件类型.</param>
    /// <param name="handler">事件函数.</param>
    public static void RemoveEvent(Protocal eventType, EventHandler handler)
    {
        lock (nMutex)
        {
            //如果字典中包含这个事件类型
            if (eventCtrlDic.ContainsKey(eventType))
            {
                //如果这个事件类型里包含这个事件
                if (eventCtrlDic[eventType].Contains(handler))
                {
                    //移除这个事件
                    eventCtrlDic[eventType].Remove(handler);
                }
            }
        }
    }
    /// <summary>
    /// 分发事件响应
    /// </summary>
    /// <param name="Type">Type.</param>
    public static  void DispensEvent(Protocal Type,byte[] bytes)
    {
        List<EventHandler> eventList = null;
        lock (nMutex)
        {
            if (eventCtrlDic.ContainsKey(Type))
            {
                eventList = eventCtrlDic[Type];
            }
        }
        if (eventList != null)
        {
            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i]?.Invoke(bytes);
            }
        }
    }
}
