using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Turn
{
    public RandomEvent myEvent;
    [SerializeField] int actionsCount = 5;
    public ModeType currentMode;

    public int ActionsCount 
    { 
        get => actionsCount; 
        set
        {
            actionsCount = value;
            EventManager.Instance.onCost?.Invoke();
        }
    }

    public Turn(int actionsCount, ModeType currentMode)
    {
        myEvent = NewEvent();
        this.actionsCount = actionsCount;
        this.currentMode = currentMode;
    }

    public RandomEvent NewEvent()
    {
        int random = UnityEngine.Random.Range(0, 100);
        foreach (var item in MainGame.Instance.LevelData.allRandomEvents)
        {
            if (random >= item.minProbabilityRange && random <= item.maxProbabilityRange && item.CanHappen())
                return item;
        }

        return null;
    }

    public bool CanChoose(int cost)
    {
        return actionsCount >= cost;
    }
}
