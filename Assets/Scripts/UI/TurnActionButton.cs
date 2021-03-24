using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnActionButton : MonoBehaviour
{
    [SerializeField] int modeIndex;
    [SerializeField] int pointCost = 1;
    Button myButton;

    private void Start()
    {
        myButton = GetComponent<Button>();
        EventManager.Instance.onCost.AddListener(UpdateUI);
    }

    public void UpdateUI()
    {
        myButton.interactable = MainGame.Instance.CurrentTurn.CanChoose(pointCost);
    }

    public void ActionCost()
    {
        UIManager.Instance.NewMode(modeIndex);
        MainGame.Instance.CurrentTurn.actionsCount -= pointCost;
        EventManager.Instance.onCost.Invoke();
    }
}
