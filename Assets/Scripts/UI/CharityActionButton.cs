using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharityActionButton : MonoBehaviour
{
    [SerializeField] CharityAction action;
    [SerializeField] Text displayDescription;

    private void Awake()
    {
        displayDescription.text = action.description;
    }

    public void ExecuteAction()
    {
        ResourceManager.Instance.GetStat(action.statType).CurrentAmount += action.amount;
        ResourceManager.Instance.ModifyMoney(-action.moneyCost);
        UIManager.Instance.ExitCharityPanel();
    }
}
