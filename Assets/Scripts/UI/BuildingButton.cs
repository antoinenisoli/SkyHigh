using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefab;
    [SerializeField] int moneyCost = 10;
    Button myButton;
    [SerializeField] Text costDisplay;
    [SerializeField] Text nameDisplay;

    private void Start()
    {
        myButton = GetComponent<Button>();
        EventManager.Instance.onCost.AddListener(UpdateUI);
        UpdateUI();
    }

    public void UpdateUI()
    {
        myButton.interactable = ResourceManager.Instance.CanBuy(moneyCost);
        costDisplay.text = -moneyCost + "$";
        nameDisplay.text = buildingPrefab.GetComponent<Building>().name;
    }

    public void GetBuilding()
    {
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * -0.1f, 0.3f);
        MainGame.Instance.BuildingPrefab = buildingPrefab;
        ResourceManager.Instance.ModifyMoney(50);
    }
}
