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
    [SerializeField] protected ParticleSystem effectVFX;
    [SerializeField] protected GameObject buildVFX;
    [SerializeField] protected StatType stat;
    [SerializeField] protected int resourceGain = 10;
    [Space(15)]
    [Tooltip("The positon that crowd members will enter and exit from. If null, they'll enter/exit from this object's position.")]
    [SerializeField] Transform crowdEntryPos = null;
    Vector3 pos;

    /// <summary>
    /// The transform containing the position crowd members should enter/exit from. <br/>
    /// If <see cref="crowdEntryPos"/> is not assigned in the inspector, this will equal this building's transform.
    /// </summary>
    public Transform CrowdEntryPosition { get { return crowdEntryPos ? crowdEntryPos : transform; } }

    public virtual void Effect()
    {
        ResourceManager.Instance.ModifyStat(stat, resourceGain);

        if (effectVFX)
            effectVFX.Play();
    }

    public void Build()
    {
        pos = transform.localPosition;
        pos.y = 0.7f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(0.4f, animDuration));
        sequence.Play().OnComplete(FinishBuilding);
        GameObject smoke = Instantiate(buildVFX, pos, buildVFX.transform.rotation, transform.parent);
        smoke.transform.localPosition = pos;

        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
    }

    public void Death()
    {
        pos = transform.localPosition;
        pos.y = 0.7f;
        transform.DOLocalMoveY(pos.y - 4, animDuration);
        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
        GameObject smoke = Instantiate(buildVFX, pos, buildVFX.transform.rotation, transform.parent);
        smoke.transform.localPosition = pos;

        Destroy(gameObject, animDuration);
        EventManager.Instance.onBuildingDestroyed?.Invoke(this);
        MainGame.Instance.AllBuildings.Remove(name + GetInstanceID());
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Death();
    }
}
