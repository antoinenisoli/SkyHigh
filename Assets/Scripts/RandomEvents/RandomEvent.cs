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
    public new string name;
    [Range(0, 100)] public int minProbabilityRange = 0;
    [Range(0, 100)] public int maxProbabilityRange = 0;
    [TextArea] public string description;
    public EventEffect[] choices = new EventEffect[2];

    public void ExecuteEvent(int index)
    {
        choices[index].Execute();
    }
}
