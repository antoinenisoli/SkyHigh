using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomEvent
{

}

public class Turn
{
    public RandomEvent myEvent;
    public int actionsCount = 5;
    public ModeType currentMode;

    public Turn(RandomEvent myEvent, int actionsCount, ModeType currentMode)
    {
        this.myEvent = myEvent;
        this.actionsCount = actionsCount;
        this.currentMode = currentMode;
    }

    public bool CanChoose(int cost)
    {
        return actionsCount >= cost;
    }
}
