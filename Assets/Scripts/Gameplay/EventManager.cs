using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    [HideInInspector] public UnityEvent onCost = new UnityEvent();

    [HideInInspector] public UnityEvent onNewTurn = new UnityEvent();
    [HideInInspector] public UnityEvent onNewAction = new UnityEvent();

    public class UnityRandomEvent : UnityEvent<RandomEvent> { }
    [HideInInspector] public UnityRandomEvent onNewRandomEvent = new UnityRandomEvent();

    public class UnityEventBool : UnityEvent<bool> { }
    [HideInInspector] public UnityEventBool onEndGame = new UnityEventBool();

    public Action<Building> onBuildingBuilt;
    [HideInInspector] public Action<CrowdMember> onCrowdMemberReachedEnd;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
