using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Hapiness,
    Education,
    Visibility,
    Money,
}

[Serializable]
public class Statistic
{
    public StatType statType;
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
