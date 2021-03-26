using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] Text globalSatisfactionDisplay;
    [SerializeField] Text displayState;
    [SerializeField] Text displayMessage;
    [SerializeField] Image seal;
    [SerializeField] Button nextButton;
    [SerializeField] Sprite[] sealSprites;
    [SerializeField] [TextArea] string[] endGameMessage = new string[6];
    Slider[] sliders;

    private void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();
    }

    private void Start()
    {
        EventManager.Instance.onEndGame.AddListener(Anim);
        seal.DOFade(0, 0);

        System.Array array = System.Enum.GetValues(typeof(StatType));
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = ResourceManager.Instance.GetStat((StatType)array.GetValue(i)).MaxAmount;
            sliders[i].value = 0;
        }
    }

    public void Anim(bool win)
    {
        StartCoroutine(SliderAnimation(win));
        transform.DOLocalMoveY(0, 1);
        displayState.color = win ? Color.green : Color.red;
        displayState.text = win ? "Victory !" : "Defeat";
    }

    IEnumerator SliderAnimation(bool b)
    {
        yield return new WaitForSeconds(1);
        System.Array array = System.Enum.GetValues(typeof(StatType));
        for (int i = 0; i < sliders.Length; i++)
        {
            float duration = 1;
            sliders[i].DOValue(ResourceManager.Instance.GetStat((StatType)array.GetValue(i)).CurrentAmount, duration);
            sliders[i].transform.DOPunchScale(Vector3.one * 0.3f, 0.2f);
            yield return new WaitForSeconds(duration + 0.4f);
        }

        string s = "<b> Global Satisfaction </b> : " + ResourceManager.Instance.GetAverage() + " %";
        globalSatisfactionDisplay.DOText(s, 1.5f);
        yield return new WaitForSeconds(1.5f);

        seal.sprite = ResourceManager.Instance.GetAverage() >= 50 ? sealSprites[0] : sealSprites[1];
        Sequence sequence = DOTween.Sequence();
        sequence.Append(seal.DOFade(1, 0.3f));
        sequence.Join(seal.transform.DOScale(Vector3.one * 0.8f, 0.3f));
        sequence.Play().OnComplete(Shake);

        string message;
        if (b)
        {
            message =
                ResourceManager.Instance.GetAverage() >= 75 ? endGameMessage[0]
                : ResourceManager.Instance.GetAverage() >= 50 ? endGameMessage[1]
                : endGameMessage[2];
        }
        else
        {
            message =
                ResourceManager.Instance.GetAverage() >= 75 ? endGameMessage[3]
                : ResourceManager.Instance.GetAverage() >= 50 ? endGameMessage[4]
                : endGameMessage[5];
        }

        displayMessage.DOText(message, 1f);
        yield return new WaitForSeconds(1);
        if (nextButton)
            nextButton.interactable = b;
    }

    void Shake()
    {
        MainGame.Instance.ShakeCamera(0.2f, 2f);
    }
}
