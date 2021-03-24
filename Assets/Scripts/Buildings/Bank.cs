using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : Building
{
    [Header("_BANK")]
    [SerializeField] int moneyGain = 10;

    public override void Effect()
    {
        ResourceManager.Instance.ModifyMoney(moneyGain);
        if (fx)
            fx.Play();
    }
}
