using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EventEffect
{
    public string name;
    public EventType eventType;
    [Header("Destroy Building")]
    public string buildingToDestroy;
    [Header("Lose or gain resource")]
    public StatType statType;
    public int amount;

    public void Execute()
    {
        if (eventType.HasFlag(EventType.LoseResource))
        {
            ResourceManager.Instance.GetStat(statType).CurrentAmount += amount;
        }

        if (eventType.HasFlag(EventType.DestroyBuilding))
        {
            Building[] b = UnityEngine.Object.FindObjectsOfType<Building>();
            foreach (var item in b)
            {
                if (item.buildingName == buildingToDestroy)
                {
                    item.Death();
                    return;
                }
            }
        }
    }
}
