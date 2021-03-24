using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Building")]
    [SerializeField] protected new string name;
    [SerializeField] protected ParticleSystem fx;

    public abstract void Effect();
}
