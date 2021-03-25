using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public Statistic[] stats = new Statistic[3];
    Dictionary<StatType, Statistic> allStats = new Dictionary<StatType, Statistic>();
    public Statistic Money;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        foreach (var item in stats)
        {
            allStats.Add(item.statType, item);
        }
    }

    public bool CanBuy(float cost)
    {
        return Money.CurrentAmount >= cost;
    }

    public Statistic GetStat(StatType type)
    {
        return allStats[type];
    }

    public void ModifyMoney(int amount)
    {
        Money.CurrentAmount += amount;
        UIManager.Instance.FloatingText(amount);
    }
}
