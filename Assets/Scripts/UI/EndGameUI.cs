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
    [SerializeField] Button nextButton;
    
    [Header("Seal")]
    [SerializeField] Image seal;
    [SerializeField] Sprite[] sealSprites;
    [SerializeField] [TextArea] string[] endGameMessage = new string[6];
    Slider[] sliders;

    private void Awake()
    {
        sliders = GetComponentsInChildren<Slider>();
    }

    private void Start()
    {
        EventManager.Instance.onEndGame.AddListener(PanelAnim);
        seal.DOFade(0, 0);
        System.Array array = System.Enum.GetValues(typeof(StatType));

        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].maxValue = ResourceManager.Instance.GetStat((StatType)array.GetValue(i)).MaxAmount;
            sliders[i].value = 0;
        }
    }

    public void PanelAnim(bool win)
    {
        SoundManager.Instance.PlayAudio("endgame");
        StartCoroutine(SliderAnimation(win));
        transform.DOLocalMoveY(0, 1).SetEase(Ease.OutBack);
        displayState.color = win ? Color.green : Color.red;
        displayState.text = win ? "Victory !" : "Defeat";
    }

    void SealAnim() //play a "seal of approval" effect on screen
    {
        seal.sprite = ResourceManager.Instance.GetGlobalSatisfaction() >= 50 ? sealSprites[0] : sealSprites[1];
        Sequence sequence = DOTween.Sequence();
        sequence.Append(seal.DOFade(1, 0.3f));
        sequence.Join(seal.transform.DOScale(Vector3.one * 0.8f, 0.3f));
        sequence.Play().OnComplete(Shake);
        SoundManager.Instance.PlayAudio("seal");
    }

    void WriteGlobalSatisfaction()
    {
        string sound = ResourceManager.Instance.GetGlobalSatisfaction() >= 50 ? "success-low" : "negative-beeps";
        SoundManager.Instance.PlayAudio(sound);
        string s = "<b> Global Satisfaction </b> : " + ResourceManager.Instance.GetGlobalSatisfaction() + " %";
        globalSatisfactionDisplay.DOText(s, 1.5f);
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
            SoundManager.Instance.PlayAudio("UI_hover");
            yield return new WaitForSeconds(duration + 0.4f);
        }

        WriteGlobalSatisfaction();
        yield return new WaitForSeconds(1.5f);
        SealAnim();

        //write a message depending on the global score at the end
        string message;
        if (b)
        {
            message =
                ResourceManager.Instance.GetGlobalSatisfaction() >= 75 ? endGameMessage[0]
                : ResourceManager.Instance.GetGlobalSatisfaction() >= 50 ? endGameMessage[1]
                : endGameMessage[2];
        }
        else
        {
            message =
                ResourceManager.Instance.GetGlobalSatisfaction() >= 75 ? endGameMessage[3]
                : ResourceManager.Instance.GetGlobalSatisfaction() >= 50 ? endGameMessage[4]
                : endGameMessage[5];
        }

        displayMessage.DOText(message, 2f);
        yield return new WaitForSeconds(1);
        if (nextButton)
            nextButton.interactable = b;
    }

    void Shake()
    {
        MainGame.Instance.ShakeCamera(0.2f, 2f);
        for (int i = 0; i < transform.root.childCount; i++)
            transform.root.GetChild(i).DOShakePosition(0.2f, 5f, 90);
    }
}
