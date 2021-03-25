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

    private void Start()
    {
        displayDescription = GetComponentInChildren<Text>();
        image = GetComponentInChildren<Image>();
        UpdateUI();
    }

    public override void UpdateUI()
    {
        if (!Action)
            return;

        button.interactable = true;
        string info = " (Earn " + Action.amount + " " + Action.statType + ", cost : " + Action.moneyCost + "$)"; 
        displayDescription.text = Action.description + info;
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
}
