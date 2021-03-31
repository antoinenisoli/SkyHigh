using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EventPanel : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text[] choicesText;
    Button[] buttons;

    private void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        EventManager.Instance.onNewRandomEvent.AddListener(UpdateUI);
    }

    public void UpdateUI(RandomEvent myEvent)
    {
        foreach (var item in buttons) { item.interactable = true; }
        nameText.text = myEvent.name;
        descriptionText.text = myEvent.description;
        for (int i = 0; i < choicesText.Length; i++)
        {
            choicesText[i].text = myEvent.choices[i].name;
        }
    }

    public void EventChoice(int choiceIndex)
    {
        SoundManager.Instance.PlayAudio("click-basic");
        MainGame.Instance.CurrentTurn.myEvent.choices[choiceIndex].Execute();
        UIManager.Instance.ExitEventPanel();
        foreach (var item in buttons) { item.interactable = false; }
    }
}
