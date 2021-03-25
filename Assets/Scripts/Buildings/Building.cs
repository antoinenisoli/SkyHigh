using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building")]
    public string buildingName;
    public Sprite buildingImage;

    public int moneyCost = 35;
    [SerializeField] float animDuration = 2f;
    [SerializeField] int shakeVibration = 90;
    [SerializeField] float shakeStrength = 0.3f;
    [SerializeField] protected ParticleSystem fx;
    [SerializeField] protected StatType stat;
    [SerializeField] protected int resourceGain = 10;
    Vector3? pos;

    public virtual void Effect()
    {
        if (stat == StatType.Money)
            ResourceManager.Instance.ModifyMoney(resourceGain);
        else
            ResourceManager.Instance.GetStat(stat).CurrentAmount += resourceGain;

        if (fx)
            fx.Play();
    }

    public void Build(Vector3 position)
    {
        transform.DOMoveY(position.y, animDuration);
        pos = position;
        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
    }

    public void Death()
    {
        transform.DOMoveY(pos.Value.y - 4, animDuration);
        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
        Destroy(gameObject, animDuration);
    }

    public override string ToString()
    {
        return "Earn " + resourceGain + " " + stat.ToString() + " per turn.";
    }
}
