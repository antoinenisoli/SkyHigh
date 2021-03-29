using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharityActionButton : ActionButton
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

    [SerializeField] Text displayName;
    [SerializeField] Text displayDescription;
    [SerializeField] Text displayEffect;
    [SerializeField] Image illustrationDisplay;

    public override void Awake()
    {
        UpdateUI();
        base.Awake();
    }

    public override void UpdateUI()
    {
        if (!Action)
            return;

        button.interactable = ResourceManager.Instance.CanBuy(Action.moneyCost);
        displayName.text = action.actionName;
        displayDescription.text = Action.description;
        displayEffect.text = ToString();
        if (Action.illustration)
            illustrationDisplay.sprite = Action.illustration;
    }

    public void ExecuteAction()
    {
        Click();
        ResourceManager.Instance.ModifyStat(Action.statType, Action.amount);
        ResourceManager.Instance.ModifyStat(StatType.Money, -Action.moneyCost);
        UIManager.Instance.ExitCharityPanel();
    }

    public override string ToString()
    {
        return "Earn " + Action.amount + " " + Action.statType + ", cost : " + MoneyConverter.Convert(Action.moneyCost) + "$";
    }
}
