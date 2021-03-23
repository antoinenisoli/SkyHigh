using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public UnityEvent onCost = new UnityEvent();

    /*public class Entity_UnityEvent : UnityEvent<Entity> { }
    public Entity_UnityEvent onNewBuilding = new Entity_UnityEvent();*/

    public class UnityEventBool : UnityEvent<bool> { }
    public UnityEventBool onEndGame = new UnityEventBool();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
