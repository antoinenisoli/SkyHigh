using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharityAction", menuName = "Action/CharityAction")]
public class CharityAction : ScriptableObject
{
    public StatType statType;
    public int amount = 20;
    public int moneyCost = 50;
    [TextArea] public string description;
}
