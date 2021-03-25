﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public UnityEvent onCost = new UnityEvent();

    public UnityEvent onNewTurn = new UnityEvent();
    public UnityEvent onNewAction = new UnityEvent();

    public class UnityRandomEvent : UnityEvent<RandomEvent> { }
    public UnityRandomEvent onNewRandomEvent = new UnityRandomEvent();

    public class UnityEventBool : UnityEvent<bool> { }
    public UnityEventBool onEndGame = new UnityEventBool();

    public Action<Building> onBuildingBuilt;
    public Action<CrowdMember> onCrowdMemberReachedEnd;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
