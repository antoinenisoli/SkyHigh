using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnActionButton : UISpecialButton
{
    [SerializeField] int modeIndex;
    [SerializeField] int pointCost = 1;

    public override void UpdateUI()
    {
        button.interactable = MainGame.Instance.CurrentTurn.CanChoose(pointCost);
    }

    public void ActionCost()
    {
        Click();
        UIManager.Instance.NewMode(modeIndex);
        MainGame.Instance.CurrentTurn.actionsCount -= pointCost;
        EventManager.Instance.onCost.Invoke();
    }
}
