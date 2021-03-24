using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] int moneyCost = 10;
    Button myButton;
    Text costDisplay;

    private void Start()
    {
        myButton = GetComponent<Button>();
        costDisplay = GetComponentInChildren<Text>();
        EventManager.Instance.onCost.AddListener(UpdateUI);
    }

    public void UpdateUI()
    {
        myButton.interactable = ResourceManager.Instance.CanBuy(moneyCost);
        costDisplay.text = moneyCost + "$";
    }

    public void GetBuilding()
    {
        MainGame.Instance.BuildingPrefab = buildingPrefab;
        ResourceManager.Instance.ModifyMoney(50);
    }
}
