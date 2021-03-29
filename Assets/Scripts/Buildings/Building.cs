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
    [Space(15)]
    [Tooltip("The positon that crowd members will enter and exit from. If null, they'll enter/exit from this object's position.")]
    [SerializeField] private Transform crowdEntryPos = null;
    Vector3? pos;

    /// <summary>
    /// The transform containing the position crowd members should enter/exit from. <br/>
    /// If <see cref="crowdEntryPos"/> is not assigned in the inspector, this will equal this building's transform.
    /// </summary>
    public Transform CrowdEntryPosition { get { return crowdEntryPos ? crowdEntryPos : transform; } }

    public virtual void Effect()
    {
        ResourceManager.Instance.ModifyStat(stat, resourceGain);

        if (fx)
            fx.Play();
    }

    public void Build()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(0.4f, animDuration));
        sequence.Play().OnComplete(FinishBuilding);

        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
    }

    public void Death()
    {
        pos = transform.position;
        transform.DOLocalMoveY(pos.Value.y - 4, animDuration);
        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
        Destroy(gameObject, animDuration);
        EventManager.Instance.onBuildingDestroyed?.Invoke(this);
    }

    void FinishBuilding()
    {
        Vector3 local = transform.localPosition;
        local.y = 0.4f;
        transform.localPosition = local;
        EventManager.Instance.onBuildingBuilt?.Invoke(this);
    }

    public override string ToString()
    {
        return "Earn " + resourceGain + " " + stat.ToString() + " per turn.";
    }
}
