using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : UISpecialButton
{
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] int moneyCost = 10;
    [SerializeField] Text costDisplay;
    [SerializeField] Text nameDisplay;

    private void Start()
    {
        EventManager.Instance.onCost.AddListener(UpdateUI);
        UpdateUI();
    }

    public override void UpdateUI()
    {
        button.interactable = ResourceManager.Instance.CanBuy(moneyCost) && !MainGame.Instance.BuildingPrefab;
        costDisplay.text = -moneyCost + "$";
        nameDisplay.text = buildingPrefab.GetComponent<Building>().name;
    }

    public void GetBuilding()
    {
        Click();
        MainGame.Instance.BuildingPrefab = buildingPrefab;
        ResourceManager.Instance.ModifyMoney(moneyCost);
        UpdateUI();
    }
}
