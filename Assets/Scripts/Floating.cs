using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] float offset = 3;
    [SerializeField] float speed;

    private void Awake()
    {
        Vector3 pos = transform.position;
        transform.DOMoveY(pos.y - offset, speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
    }
}
