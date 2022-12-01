using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : Singleton<EventHandler>
{
    public delegate void ApplyEvent();
    public delegate void ApplyEvent<T>(T prarm1);
    public delegate void ApplyEvent<T, P>(T prarm1, P param2);
    public delegate void ApplyEvent<T, P, M>(T prarm1, P param2, M param3);
    public delegate void ApplyEvent<T, P, M, N>(T prarm1, P param2, M param3, N param4);

    private static Dictionary<EventMsg, Delegate> m_eventPool = new Dictionary<EventMsg, Delegate>();

    public static void Invoke(EventMsg eventType)
    {
        Delegate eventValue = null;
        if (m_eventPool.TryGetValue(eventType, out eventValue)) {
            ApplyEvent applyEvent = eventValue as ApplyEvent;
            applyEvent();
        } else {
            Debug.LogError(eventType.ToString() + "is not exist !");
        }
    }

    public static void Add(EventMsg eventType, ApplyEvent eventValue)
    {
        if (m_eventPool.ContainsKey(eventType)) {
            m_eventPool[eventType] = (ApplyEvent)m_eventPool[eventType] + eventValue;
        } else {
            m_eventPool.Add(eventType, eventValue);
        }
    }

    public static void Remove(EventMsg eventType, ApplyEvent eventValue)
    {
        if (m_eventPool.ContainsKey(eventType)) {
            m_eventPool[eventType] = (ApplyEvent)m_eventPool[eventType] - eventValue;

            if (m_eventPool[eventType] == null) {
                m_eventPool.Remove(eventType);
            }
        }

    }

}
