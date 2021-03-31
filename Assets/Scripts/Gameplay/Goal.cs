using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Condition
{
    Equal,
    Greater,
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
        // check if the condition is valid
        Statistic stat = ResourceManager.Instance.GetStat(this.stat);
        switch (condition)
        {
            case Condition.Equal:
                return stat.CurrentAmount == compare;
            case Condition.Greater:
                return stat.CurrentAmount >= compare;
            case Condition.Lower:
                return stat.CurrentAmount < compare;
        }

        return false;
    }

    public override string ToString()
    {
        return "- " + stat.ToString() +
            " needs to be " + condition.ToString().ToLower() +
            " than " + compare.ToString().ToLower();
    }
}
