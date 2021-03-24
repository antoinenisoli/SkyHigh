using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Resource
{
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

    public Resource Money;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool CanBuy(float cost)
    {
        return Money.CurrentAmount >= cost;
    }

    public void Cost(int amount)
    {
        Money.CurrentAmount -= amount;
        UIManager.Instance.FloatingText("-" + amount + " $");
    }
}
