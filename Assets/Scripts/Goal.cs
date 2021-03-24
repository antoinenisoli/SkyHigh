using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Condition
{
    Equal,
    Higher,
    Lower,
}

[Serializable]
public class Goal
{
    public StatType stat;
    public Condition condition;
    public int compare = 50;

    public bool Check()
    {
        Statistic stat = ResourceManager.Instance.GetStat(this.stat);
        switch (condition)
        {
            case Condition.Equal:
                return stat.CurrentAmount == compare;
            case Condition.Higher:
                return stat.CurrentAmount > compare;
            case Condition.Lower:
                return stat.CurrentAmount < compare;
        }

        return false;
    }
}
