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
    public string currentAction = "Build mode";

    public Turn(RandomEvent myEvent, int actionsCount, string currentAction)
    {
        this.myEvent = myEvent;
        this.actionsCount = actionsCount;
        this.currentAction = currentAction;
    }
}
