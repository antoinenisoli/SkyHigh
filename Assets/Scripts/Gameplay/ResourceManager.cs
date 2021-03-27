using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Range(0,100)] public int hapinessPercent = 10;
    [Range(0, 100)] public int educationPercent = 35;
    public Statistic[] stats = new Statistic[3];
    Dictionary<StatType, Statistic> allStats = new Dictionary<StatType, Statistic>();
    public Statistic Money;

    public int BaseIncome => GetStat(StatType.Education).CurrentAmount * educationPercent / 100;

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

    public int GetGlobalSatisfaction()
    {
        int i = 0;
        foreach (var item in allStats.Values)
            i += item.CurrentAmount;

        return i / allStats.Count;
    }

    public void PayDay()
    {
        ModifyStat(StatType.Money, BaseIncome);
    }

    public bool CanBuy(float cost)
    {
        float negative = cost > 0 ? -cost : cost;
        return Money.CurrentAmount + negative >= 0;
    }

    public Statistic GetStat(StatType type)
    {
        if (allStats.ContainsKey(type))
            return allStats[type];

        return null;
    }

    public void ModifyStat(StatType type, int amount)
    {
        if (type == StatType.Money)
        {
            int computeAmount = amount;
            if (amount > 0)
            {
                int percent = GetStat(StatType.Hapiness).CurrentAmount * hapinessPercent / 100;
                computeAmount += percent;
            }

            Money.CurrentAmount += computeAmount;
            UIManager.Instance.FloatingText(computeAmount);
        }
        else
        {
            GetStat(type).CurrentAmount += amount;
        }
    }
}
