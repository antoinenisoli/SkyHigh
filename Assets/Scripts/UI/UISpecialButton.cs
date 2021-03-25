using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UISpecialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 startScale;
    protected Button button;

    public void Awake()
    {
        startScale = transform.localScale;
        button = GetComponentInChildren<Button>();
    }

    public abstract void UpdateUI();

    public void Click()
    {
        transform.DOComplete();
        transform.DOPunchScale(Vector3.one * -0.2f, 0.3f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        transform.DOScale(startScale * 1.1f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        transform.DOScale(startScale, 0.2f);
    }
}
