using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Resource
{
    public string type;
    public Text displayResource;
    [SerializeField] int currentAmount;
    [SerializeField] int maxAmount;

    public int CurrentAmount
    {
        get => currentAmount;

        set
        {
            if (value > maxAmount)
                value = maxAmount;

            if (value < 0)
                value = 0;

            currentAmount = value;
            EventManager.Instance.onCost.Invoke();
        }
    }

    public int MaxAmount { get => maxAmount; set => maxAmount = value; }
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] Resource[] resources;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        EventManager.Instance.onCost.AddListener(UpdateUI);
        foreach (var item in resources)
        {
            item.CurrentAmount = item.MaxAmount;
        }
    }

    public void Cost(int amount, int resourceIndex)
    {
        if (resources.Length > resourceIndex && resources[resourceIndex] != null)
        {
            resources[resourceIndex].CurrentAmount -= amount;
            UIManager.Instance.FloatingText(resources[resourceIndex].displayResource.transform.position, "-" + amount + " $");
        }
    }

    public void UpdateUI()
    {
        foreach (var item in resources)
        {
            item.displayResource.text = item.CurrentAmount + " $";
        }
    }
}
