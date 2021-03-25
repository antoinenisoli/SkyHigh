using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CharityActionButton : MonoBehaviour
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

    Button button;
    Text displayDescription;
    Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        displayDescription = GetComponentInChildren<Text>();
        image = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
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
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * -0.2f, 0.3f);
        ResourceManager.Instance.GetStat(Action.statType).CurrentAmount += Action.amount;
        ResourceManager.Instance.ModifyMoney(-Action.moneyCost);
        UIManager.Instance.ExitCharityPanel();
    }
}
