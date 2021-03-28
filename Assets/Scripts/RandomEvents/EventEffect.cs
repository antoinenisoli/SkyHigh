using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventEffect
{
    [Serializable]
    struct StatChange
    {
        public StatType statType;
        public int amount;
    }

    public string name;
    public EventType eventType;
    [Header("Destroy Building")]
    public string buildingToDestroy;
    [Header("Lose or gain resource")]
    [SerializeField] StatChange[] changes;

    public void Execute()
    {
        if (changes.Length > 0)
        {
            foreach (var item in changes)
                ResourceManager.Instance.ModifyStat(item.statType, item.amount);
        }

        if (eventType.HasFlag(EventType.DestroyBuilding))
        {
            foreach (var item in MainGame.Instance.AllBuildings)
            {
                if (item.Value.buildingName == buildingToDestroy)
                {
                    item.Value.Death();
                    return;
                }
            }
        }
    }

    public bool CanHappen()
    {
        bool b = true;
        if (eventType.HasFlag(EventType.DestroyBuilding))
        {
            b = false;
            foreach (var item in MainGame.Instance.AllBuildings)
            {
                if (item.Key.Contains(buildingToDestroy))
                {
                    b = true;
                    break;
                }
            }
        }

        return b;
    }
}
