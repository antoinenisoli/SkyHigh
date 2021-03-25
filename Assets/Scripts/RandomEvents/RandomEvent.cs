using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum EventType
{
    DestroyBuilding,
    LoseResource,
}

[CreateAssetMenu(fileName = "NewEvent", menuName = "Random Events/New Event")]
public class RandomEvent : ScriptableObject
{
    public EventType eventType;
    [Range(0, 100)] public int minProbabilityRange = 0;
    [Range(0, 100)] public int maxProbabilityRange = 0;

    [Header("Destroy Building")]
    public string buildingToDestroy;
    [Header("Lose or gain resource")]
    public StatType statType;
    public int amount;

    public void ExecuteEvent()
    {
        if (eventType.HasFlag(EventType.LoseResource))
        {
            ResourceManager.Instance.GetStat(statType).CurrentAmount += amount;
        }
        else if (eventType.HasFlag(EventType.DestroyBuilding))
        {
            Building[] b = FindObjectsOfType<Building>();
            foreach (var item in b)
            {
                if (item.name == buildingToDestroy)
                {
                    Destroy(item.gameObject);
                    return;
                }
            }
        }

        MonoBehaviour.print(name);
    }
}
