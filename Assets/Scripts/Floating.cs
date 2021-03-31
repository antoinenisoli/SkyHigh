using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float offset = 3f;
    [SerializeField] float animDuration = 3f;

    void Start()
    {
        // do a simple floating custom animation
        Vector3 startPos = transform.position;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(startPos.y - offset, animDuration/2).SetEase(Ease.InOutSine));
        sequence.Append(transform.DOMoveY(startPos.y, animDuration/2).SetEase(Ease.InOutSine));
        sequence.Play().SetLoops(-1);
    }
}
