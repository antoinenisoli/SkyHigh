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
    Cell myCell;

    [Header("Shake")]
    [SerializeField] float animDuration = 2f;
    [SerializeField] int shakeVibration = 90;
    [SerializeField] float shakeStrength = 0.3f;

    [Header("Feedbacks")]
    [SerializeField] float heightOffset = -0.4f;
    [SerializeField] protected ParticleSystem effectVFX;
    [SerializeField] protected GameObject buildVFX;

    [Header("Effect")]
    [SerializeField] protected StatType stat;
    [SerializeField] protected int resourceGain = 10;

    [Header("Crowd")]
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
        effectVFX?.Play();
    }

    public void Build(Cell cell) //place the building at the defined height, and do a tween to move it above the island soil
    {
        myCell = cell;
        pos = transform.localPosition;
        pos.y = heightOffset;
        transform.localPosition = pos;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveY(0.4f, animDuration));
        sequence.Play().OnComplete(FinishBuilding);

        pos.y = 0.7f;
        GameObject smoke = Instantiate(buildVFX, pos, buildVFX.transform.rotation, transform.parent);
        smoke.transform.localPosition = pos;
        SoundManager.Instance.PlayAudio("earth-rumble", true, animDuration);

        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
    }

    public void Death() //move the building beneath the island and destroy it
    {
        pos = transform.localPosition;
        transform.DOLocalMoveY(-5, animDuration);
        MainGame.Instance.ShakeCamera(animDuration, shakeStrength, shakeVibration);
        pos.y = 0.7f;
        GameObject smoke = Instantiate(buildVFX, pos, buildVFX.transform.rotation, transform.parent);
        smoke.transform.localPosition = pos;
        SoundManager.Instance.PlayAudio("earth-rumble", true, animDuration);

        Destroy(gameObject, animDuration);
        EventManager.Instance.onBuildingDestroyed?.Invoke(this);
        MainGame.Instance.AllBuildings.Remove(name + GetInstanceID());
        myCell.full = false;
        myCell.HighLight();
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
        //write the building effect's per turn
        string gain = stat == StatType.Money ? MoneyConverter.Convert(resourceGain) : resourceGain.ToString();
        return "Earn " + gain + " " + stat.ToString() + " per turn.";
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
            Death();
#endif
    }
}
