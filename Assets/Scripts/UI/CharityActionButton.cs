using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharityActionButton : UISpecialButton
{
    CharityAction action;
    public CharityAction Action 
    {
        get => action;
        set
        {
            action = value;
            UpdateUI();
        }
    }

    Text displayDescription;
    Image image;

    public override void Awake()
    {
        displayDescription = GetComponentInChildren<Text>();
        image = GetComponentInChildren<Image>();
        UpdateUI();
        base.Awake();
    }

    public override void UpdateUI()
    {
        if (!Action)
            return;

        button.interactable = true;
        displayDescription.text = Action.description + ToString();
        if (Action.illustration)
            image.sprite = Action.illustration;
    }

    public void ExecuteAction()
    {
        Click();
        ResourceManager.Instance.GetStat(Action.statType).CurrentAmount += Action.amount;
        ResourceManager.Instance.ModifyMoney(-Action.moneyCost);
        UIManager.Instance.ExitCharityPanel();
    }

    public override string ToString()
    {
        return "\n (Earn " + Action.amount + " " + Action.statType + ", cost : " + Action.moneyCost + "$)";
    }
}
