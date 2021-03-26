using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] [Range(0,100)] int hapinessPercent = 10;
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

    public int GetAverage()
    {
        int i = 0;
        foreach (var item in allStats.Values)
            i += item.CurrentAmount;

        return i / allStats.Count;
    }

    public bool CanBuy(float cost)
    {
        return Money.CurrentAmount + cost >= 0;
    }

    public Statistic GetStat(StatType type)
    {
        return allStats[type];
    }

    public void ModifyMoney(int amount)
    {
        int computeAmount = amount;
        if (amount > 0)
        {
            int percent = GetStat(StatType.Hapiness).CurrentAmount * hapinessPercent / 100;
            print(percent);
            computeAmount += percent;
        }

        Money.CurrentAmount += computeAmount;
        UIManager.Instance.FloatingText(computeAmount);
    }
}
