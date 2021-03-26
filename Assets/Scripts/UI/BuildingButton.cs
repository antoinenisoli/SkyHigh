using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : ActionButton
{
    [HideInInspector] public GameObject buildingPrefab;
    Building building;
    [SerializeField] Image photo;
    [SerializeField] Text costDisplay;
    [SerializeField] Text nameDisplay;
    [SerializeField] Text descriptionDisplay;

    public override void Start()
    {
        base.Start();
        building = buildingPrefab.GetComponent<Building>();
        UpdateUI();
    }

    public override void UpdateUI()
    {
        button.interactable = ResourceManager.Instance.CanBuy(building.moneyCost) && !MainGame.Instance.BuildingPrefab;
        costDisplay.text = -building.moneyCost + "$";
        nameDisplay.text = building.name;
        descriptionDisplay.text = building.ToString();
        if (building.buildingImage)
            photo.sprite = building.buildingImage;
    }

    public void GetBuilding()
    {
        Click();
        MainGame.Instance.BuildingPrefab = buildingPrefab;
        ResourceManager.Instance.ModifyStat(StatType.Money, building.moneyCost);
        UpdateUI();
    }
}
