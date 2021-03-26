using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionButton : UISpecialButton
{
    [SerializeField] protected TurnActionButton myTurnAction;

    public override void Click()
    {
        base.Click();
        if (myTurnAction)
            MainGame.Instance.CurrentTurn.actionsCount -= myTurnAction.pointCost;
    }
}
