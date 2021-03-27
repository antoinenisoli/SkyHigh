using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharityAction", menuName = "Action/CharityAction")]
public class CharityAction : ScriptableObject
{
    public string actionName;
    public StatType statType;
    public Sprite illustration;
    public int amount = 20;
    public int moneyCost = 50;
    [TextArea(15,15)] public string description;
}
