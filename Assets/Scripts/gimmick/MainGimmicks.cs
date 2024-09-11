using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using UnityEngine.Events;

[System.Serializable]
public class EventData : UnityEvent
{
    [SerializeField] private string eventName;
    [SerializeField] private int eventID;
    [SerializeField] private float delay;

    public string EventName
    {
        get { return eventName; }
        set { eventName = value; }
    }

    public int EventID
    {
        get { return eventID; }
        set { eventID = value; }
    }

    public float Delay
    {
        get { return delay; }
        set { delay = value; }
    }
}

[System.Serializable]
public class EventDatar : EventData
{

}

public class MainGimmicks : MonoBehaviour
{
    public List<EventDatar> OutputEvent;
}


#if UNITY_EDITOR
public class MainGimmicksEditor : Editor
{

}
#endif